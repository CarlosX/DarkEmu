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

#ifndef _SREMU_LUA_H_
#define _SREMU_LUA_H_

#pragma warning( disable: 4251 )

#include "../common.h"
#include "../log.h"
#include "../threading/sync.h"

extern "C" {
	#include "include/lua.h"
	#include "include/lualib.h"
	#include "include/lauxlib.h"
}

class SREMU_SHARED LuaManager {
public:
	LuaManager(Log* logger=0);
	~LuaManager();

	// Runs a script
	bool exec(const char* script);

	// Prints the status of the manager
	void status(int whichfunc=0);

protected:
	template <typename T>
	friend class Luna;

	struct generic_entry {
		const char* name;
		void* ptr;
	};

	bool oninject(const char* class_name,void* ptr,const char* gname);
	bool onbind(const char* class_name,generic_entry* method_table,bool instantiable);
	lua_State* get_state(){return lua;}

private:
	FastMutex bindlock;

	typedef std::map< const char*, void* > instance_map;
	typedef std::pair< const char*, void* > instance_pair;
	typedef std::map< void*, const char* > rinstance_map;
	typedef std::pair< void*, const char* > rinstance_pair;

	struct bind_data{
		generic_entry* method_table;
		int num_funcs;
		bool instantiable;

		instance_map instances;
		rinstance_map rinstances;
	};

	typedef std::map< const char*, bind_data > bind_map;
	typedef std::pair< const char*, bind_data > bind_pair;
	bind_map bound_classes;			// List of all classes we've exposed to Lua

	lua_State* lua;
	Log* log;
};

// Custom modded up Luna binder
template <typename T>
class SREMU_SHARED Luna {
	Luna();
public:
	typedef struct { T* p; } data_type;
	typedef T class_type;

	typedef int (T::*mypfn)(lua_State* L);
	
	struct method_s {
		char* name;
		mypfn callback;
	};

	// "Inject" method used explicitly by the lua manager for injecting an instance of this class globally
	static bool inject(LuaManager* mgr,T* thisptr,const char* name) {
		lua_State* L = mgr->get_state();
		
		if(!mgr->oninject(T::lua_class_name,(void*)thisptr,name))return false;

		data_type* userdata = (data_type*)lua_newuserdata(L,sizeof(data_type));
		userdata->p = thisptr;
		luaL_getmetatable(L,T::lua_class_name);
		lua_setmetatable(L,-2);
		lua_setglobal(L,name);
		return true;
	}

	// bind method which binds this class type to lua
	static bool bind(LuaManager* mgr){
		lua_State* L = mgr->get_state();

		if(!mgr->onbind(T::lua_class_name,(LuaManager::generic_entry*)T::lua_methods,T::lua_instantiable))return false;

		// Create a new lua table to store the methods
		lua_newtable(L);
		int method_tbl = lua_gettop(L);

		// Create the metatable that is a template for all objects
		luaL_newmetatable(L,T::lua_class_name);
		int metatable = lua_gettop(L);

		// Store the method table globally for lua-functions (overriding, extending, etc)
		lua_pushstring(L,T::lua_class_name);
		lua_pushvalue(L,method_tbl);
		lua_settable(L,LUA_GLOBALSINDEX);

		lua_pushliteral(L,"__metatable");
		lua_pushvalue(L,method_tbl);
		lua_settable(L,metatable);

		lua_pushliteral(L,"__index");
		lua_pushvalue(L,method_tbl);
		lua_settable(L,metatable);

		lua_pushliteral(L,"__tostring");
		lua_pushcfunction(L,tostring);
		lua_settable(L,metatable);

		if(T::lua_instantiable){
			lua_pushliteral(L,"__gc");
			lua_pushcfunction(L,del);
			lua_settable(L,metatable);

			// Make the method table
			lua_newtable(L);
			int mt = lua_gettop(L);
			lua_pushliteral(L,"__call");
			lua_pushcfunction(L,makenew);
			lua_pushliteral(L,"new");
			lua_pushvalue(L,-2);				// Duplicate the fn ptr
			lua_settable(L,method_tbl);		// Add new_T to method table
			lua_settable(L,mt);					// mt.__call = new_T
			lua_setmetatable(L,method_tbl);	// set the metatable
		}

		// Construct the method table now
		for(method_s* pmethod=T::lua_methods; pmethod->name;pmethod++){
			lua_pushstring(L,pmethod->name);			// Push the name
			lua_pushlightuserdata(L,(void*)pmethod);	// Set the upvalue for this method call with 'pmethod->name'
			lua_pushcclosure(L,dispatch,1);				// Push the dispatch function
			lua_settable(L,method_tbl);				// Add the method to the table
		}

		// Remove the metatable and method table
		lua_pop(L,2);
		return true;
	}

protected:

	// Static methods
	// Check returns the 'thisptr' from the stack at narg
	static T* check(lua_State* L,int narg){
		data_type* userdata = (data_type*)luaL_checkudata(L,narg,T::lua_class_name);
		//if(!userdata) luaL_typerror(L,narg,class_name);
		if(!userdata) return 0;
		return userdata->p;
	}

	// Function dispatcher
	static int dispatch(lua_State* L) {
		T* thisptr = check(L,1);
		if(thisptr==0)return 0;		// Return without calling, invalid thisptr on stack
		lua_remove(L,1);			// Adjust stack for callee

		// Grab the method being invoked
		method_s* method = (method_s*)lua_touserdata(L,lua_upvalueindex(1));
		return (thisptr->*(method->callback))(L);
	}

	// "New" method for making new objects in lua, only registered if
	// this object declares that it wants to be instantiable instead of static in lua.
	static int makenew(lua_State* L) {
		lua_remove(L,1);					// Remove the userdata on top of the stack
		T* thisptr = new T(L);				// Call the constructor for new objects
		data_type* userdata = (data_type*)lua_newuserdata(L,sizeof(data_type));		// Instantiate new userdata object
		userdata->p = thisptr;				// Store the new pointer
		luaL_getmetatable(L,T::lua_class_name);	// Lookup the metatable
		lua_setmetatable(L,-2);				// Store it for this userdata type
		return 1;							// Returns the new userdata
	}

	// "delete" method in a sense, called by garbage collector to clean up a userdata
	static int del(lua_State* L){
		data_type* userdata = (data_type*)lua_touserdata(L,1);
		T* thisptr = userdata->p;
		delete thisptr;
		return 0;
	}

	// "tostring" method for the object
	static int tostring(lua_State* L){
		data_type* userdata = (data_type*)lua_touserdata(L,1);
		T* thisptr = userdata->p;
		lua_pushfstring(L,"%s (%p)",T::lua_class_name,thisptr);
		return 1;
	}
};

// TODO: Implement this LuaInterface, atm we're using raw calls which is nasteh
/*

class SREMU_SHARED LuaInterface {
public:

	LuaInterface(lua_State* L,bool del_on_destroy=false):lua(L),del(del_on_destroy){}
	~LuaInterface(){
		if(del)
			lua_close(lua);
	}

	// TODO: Implement better lua api here, abstract the all-important stack away
	// and make accessing/type marshalling simpler

private:
	bool del;
	lua_State* lua;
};

*/

#define LUA_CLASS_DECLARE(x) x(lua_State* L); \
	static char lua_class_name[]; \
	static Luna<x>::method_s lua_methods[]; \
	static bool lua_instantiable

#define LUA_CLASS_DEFINE(x,y,z) char x::lua_class_name[] = #y; \
	bool x::lua_instantiable=z?true:false

#define LUA_CALLTABLE_BEGIN(x)			Luna<x>::method_s x::lua_methods[] = {
#define LUA_CALLTABLE_DECLARE(x,y,z)	{ #y , (Luna<x>::mypfn)&x::z },
#define LUA_CALLTABLE_END				{ 0, 0 } };

#endif // #ifndef _SREMU_LUA_H_