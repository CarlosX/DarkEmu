 /* 
  * This file is part of SREmu.
  *
  * Copyright (c) 2010 Justin "jMerliN" Summerlin, SREmu <http://sremu.sourceforge.net>
  *
  * SREmu is free software: you can redistribute it and/or modify
  * it under the terms of the GNU Affero General Public License as published by
  * the Free Software Foundation, either version 3 of the License, or
  * (at your option) any later version.
  * 
  * SREmu is distributed in the hope that it will be useful,
  * but WITHOUT ANY WARRANTY; without even the implied warranty of
  * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  * GNU Affero General Public License for more details.
  * 
  * You should have received a copy of the GNU Affero General Public License
  * along with SREmu.  If not, see <http://www.gnu.org/licenses/>.
  */

enum CLIENT_OPCODES 
{
	AGENT_CLIENT_INFO					= 0x2001,	// g
	AGENT_CLIENT_KEEP_ALIVE				= 0x2002,	// g
	AGENT_CLIENT_PATCH_REQUEST			= 0x6100,	// g
	AGENT_CLIENT_AUTH					= 0x6103,	// g
	AGENT_CLIENT_ITEM_MOVE				= 0x706d,
	AGENT_CLIENT_INGAME_NOTIFY			= 0x707b,
	AGENT_CLIENT_CLOSE					= 0x70b7,
	AGENT_CLIENT_CHARACTER				= 0x7007,	// g
	AGENT_CLIENT_CHAT					= 0x7367,
	AGENT_CLIENT_PLAY					= 0x7001,	// g
	AGENT_CLIENT_TARGET					= 0x745A,
	AGENT_CLIENT_GM						= 0x75b6,
	AGENT_CLIENT_MOVEMENT				= 0x7738,
	AGENT_CLIENT_ACCEPT_HANDSHAKE 		= 0x9000,	// g

	GATEWAY_CLIENT_INFO					= 0x2001,	// g
	GATEWAY_CLIENT_KEEP_ALIVE			= 0x2002,	// g
	GATEWAY_CLIENT_PATCH_REQUEST		= 0x6100,	// g
	GATEWAY_CLIENT_SERVERLIST_REQUEST	= 0x6101,	// g
	GATEWAY_CLIENT_AUTH					= 0x6102,	// g
	GATEWAY_CLIENT_ACCEPT_HANDSHAKE 	= 0x9000,	// g
	GATEWAY_CLIENT_LAUNCHER				= 0x6104	// g
};

enum SERVER_OPCODES 
{
	AGENT_SERVER_INFO				= 0x2001,	// g
	AGENT_SERVER_SPAWN				= 0x30D7,	/* This is actually used for all objects (items, players) */
	AGENT_SERVER_QUIT_GAME			= 0x315A,
	AGENT_SERVER_LOADING_END		= 0x31DB,
	AGENT_SERVER_CHARDATA			= 0x32B3,
	AGENT_SERVER_GUILD_UPDATE		= 0x32C4,
	AGENT_SERVER_ITEM_EQUIP			= 0x3314,
	AGENT_SERVER_STATS				= 0x343C,
	AGENT_SERVER_GUILD_UNKNOWN1		= 0x365F,
	AGENT_SERVER_CHAT				= 0x3667,
	AGENT_SERVER_DESPAWN_PLAYER		= 0x36AB,
	AGENT_SERVER_ITEM_UNEQUIP		= 0x377C,
	AGENT_SERVER_LOADING_START		= 0x379D,
	AGENT_SERVER_HANDSHAKE			= 0x5000,
	AGENT_SERVER_PATCH_INFO			= 0x600D,
	AGENT_SERVER_ITEM_MOVEMENT		= 0xB06D,
	AGENT_SERVER_AUTH				= 0xA103,
	AGENT_SERVER_COUNTDOWN			= 0xB0B7,
	AGENT_SERVER_CHARACTER			= 0xB007,	// g
	AGENT_SERVER_CHAT_ACCEPT		= 0xB367,
	AGENT_SERVER_PLAY				= 0xB001,	// g
	AGENT_SERVER_TARGET				= 0xB45A,
	AGENT_SERVER_GUILD_INFO			= 0xB6B8,
	AGENT_SERVER_MOVEMENT			= 0xB738,

	GATEWAY_SERVER_INFO				= 0x2001,	// g
	GATEWAY_SERVER_HANDSHAKE		= 0x5000,	// g
	GATEWAY_SERVER_PATCH_INFO		= 0x600D,	// g
	GATEWAY_SERVER_LAUNCHER			= 0x600D,	// g
	GATEWAY_SERVER_LIST				= 0xA101,	// g
	GATEWAY_SERVER_AUTH_INFO		= 0xA102	// g
};