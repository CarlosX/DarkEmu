/* 
 * Copyright (C) 2005-2008 SREmu <http://www.sremu.org/>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#include "lua.h"

LuaManager::LuaManager(Log* logger):log(logger),lua(0){
	// Open lua state object
	lua = lua_open();

	if(!lua && log){
		log->print("LuaManager: Error creating lua state object!\n");
	}else{
		// Open all lua libs
		luaL_openlibs(lua);
		if(log)
			log->print("LuaManager: Lua manager started!\n");
	}
}

LuaManager::~LuaManager(){
	// Close the lua state object
	if(lua)
		lua_close(lua);

	if(log)
		log->print("LuaManager: Lua manager shutdown!\n");
}

void LuaManager::status(int whichfunc){
	if(!log)return;	// Can't print status!  luls!

	if(whichfunc>2 || whichfunc<0)return;	// Invalid function!

	typedef void (Log::*log_func)(const char* fmt,...);
	log_func mylog[] = { &Log::print, &Log::echo, &Log::log };

	// We need a way to "lock" the logger, maybe another fast mutex?
	// TODO: make a listing system in the logger to protect what we're about to do :o
	bindlock.lock();
	(log->*mylog[whichfunc])("LuaManager{state}: There are currently %d classes bound.\n",bound_classes.size());

	int j=1;
	for(bind_map::iterator i=bound_classes.begin();i!=bound_classes.end();i++,j++){
		bind_data* dt = &i->second;
		(log->*mylog[whichfunc])("%d: \"%s\"(%s), has %d exported functions:\n",j,i->first,dt->instantiable?"instantiable":"non-instantiable",dt->num_funcs);

		if(dt->num_funcs>0){
			for(int k=0;k<dt->num_funcs;k++){
				(log->*mylog[whichfunc])("\t%d: \"%s\" @ %p\n",k+1,dt->method_table[k].name,dt->method_table[k].name);
			}
		}

		if(dt->instances.size()){
			(log->*mylog[whichfunc])("Instance list(%d):\n",dt->instances.size());

			int k=1;
			for(std::map< const char*,void* >::iterator i=dt->instances.begin();i!=dt->instances.end();i++,k++)
				(log->*mylog[whichfunc])("\t%d: Lua global name \"%s\" instance @ %p\n",k,i->first,i->second);
		}
	}

	(log->*mylog[whichfunc])("LuaManager{state}: End listing.\n");

	bindlock.unlock();
}

bool LuaManager::exec(const char* script){
	if(!lua)return false;
	if(luaL_loadfile(lua,script)||lua_pcall(lua,0,0,0)){
		if(log)
			log->print("LuaManager{exec}: Error starting script %s, error from system:\n%s\n",script,lua_tostring(lua,-1));
		return false;
	}

	return true;
}

bool LuaManager::oninject(const char* class_name,void* ptr,const char* gname){
	if(!lua)return false;

	if(log)
		log->print("LuaManager: Injecting global instance of \"%s\" at 0x%.8X to lua as global named \"%s\"!\n",class_name,ptr,gname);

	bindlock.lock();

	// Check if we have bound this class type
	bind_map::iterator m=bound_classes.find(class_name);
	if(m==bound_classes.end()){
		if(log)
			log->print("LuaManager: Injection failed.  The class type %s has not been bound to Lua!\n",class_name);
		bindlock.unlock();
		return false;
	}

	// Check if we've already injected this object
	bind_data* bd = &m->second;
	instance_map::iterator i=bd->instances.find(gname);

	if(i!=bd->instances.end()){
		if(log)
			log->print("LuaManager: Injection failed.  An instance has already been injected with this global name!\n");
		bindlock.unlock();
		return false;
	}

	rinstance_map::iterator j=bd->rinstances.find(ptr);

	if(j!=bd->rinstances.end()){
		if(log)
			log->print("LuaManager: Injection failed.  This specific instances has already been injected (\"%s\")!\n",j->second);
		bindlock.unlock();
	}

	// Check if there's a global named this in lua already
	lua_getglobal(lua,gname);
	if(!lua_isnil(lua,-1)){
		lua_pop(lua,1);
		if(log)
			log->print("LuaManager: Injecting failed!  This global name already exists!\n");
		bindlock.unlock();
		return false;
	}

	lua_pop(lua,1);

	// Add this object to the instances maps
	bd->instances.insert(instance_pair(gname,ptr));
	bd->rinstances.insert(rinstance_pair(ptr,gname));

	bindlock.unlock();

	return true;
}

bool LuaManager::onbind(const char* class_name,generic_entry* method_table,bool instantiable){
	if(!lua)return false;	// Can't bind if the Lua instance is bad!  lol.

	if(log)
		log->print("LuaManager: Bind new class named %s!\n",class_name);

	bindlock.lock();
	
	// Ensure this object is not bound already...
	bind_map::iterator m=bound_classes.find(class_name);
	if(m!=bound_classes.end()){
		if(log)
			log->print("LuaManager: Binding failed!  This class has already been bound!\n");
		bindlock.unlock();
		return false;
	}

	// Insert the object to the bind list
	int size=0;
	generic_entry* p=method_table;
	while(p->name){
		size++;
		p++;
	}
	bind_data bd = {method_table,size,instantiable};
	bound_classes.insert(bind_pair(class_name,bd));

	bindlock.unlock();

	return true;

}