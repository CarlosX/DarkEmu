#pragma once

#ifndef SILKROAD_SECURITY_H_
#define SILKROAD_SECURITY_H_

//-----------------------------------------------------------------------------

#include "shared_types.h"
#include "stream_utility.h"
#include <vector>
#include <string>

//-----------------------------------------------------------------------------

struct PacketContainer
{
	uint16_t opcode;
	StreamUtility data;
	uint8_t encrypted;
	uint8_t massive;

	PacketContainer();
	PacketContainer( uint16_t packet_opcode, const StreamUtility & packet_data, uint8_t packet_encrypted, uint8_t packet_massive );
	PacketContainer( const PacketContainer & rhs );
	PacketContainer & operator =( const PacketContainer & rhs );
	~PacketContainer();
};

//-----------------------------------------------------------------------------

struct SilkroadSecurityData;
class SilkroadSecurity
{
public:
	SilkroadSecurityData * m_data;

private:
	SilkroadSecurity( const SilkroadSecurity & rhs );
	SilkroadSecurity & operator =( const SilkroadSecurity & rhs );

public:
	SilkroadSecurity();
	~SilkroadSecurity();

	// Only call this function if this security object is being used in a server
	// context and not a clientless. Be sure to run HasPacketToSend and 
	// GetPacketToSend logic afterwards on connect to send the initial packet.
	void GenerateHandshake( uint8_t blowfish = true, uint8_t security_bytes = true, uint8_t handshake = true );

	// Allows you to change the identity sent after the handshake has been
	// accepted. This function is only for advanced users and does not need
	// to be used for normal operations. Defaults to "SR_Client" and 0.
	void ChangeIdentity				 ( const std::string & name, uint8_t flag );
	const std::string & GetIdentify  ();

	// Transfers raw incoming data into the security object. After calling, you
	// should check to see if there are any packets ready for processing via
	// the HasPacketToRecv function and if so, dispatch them by getting them
	// via the GetPacketToRecv function. Do not call with non-network data as 
	// the stream can corrupt! This is a very heavy function that should
	// only be called within a serialized network thread. Can throw.
	void Recv( const uint8_t * stream, int32_t count );
	void Recv( const std::vector< uint8_t > & stream );

	// Returns true if there are any packets ready to be processed. This function
	// should be called after Recv or at some regular interval depending 
	// on your implementation.
	uint8_t HasPacketToRecv() const;

	// Returns the next available packet that has been extracted from the stream.
	// NOTE: Certain packets are processed internally and then returned to the user
	// in a usable form. For example: 0x600D packets are handled internally and the
	// data they contain is returned through this function. You will never get a
	// a 0x600D packet to process. Can throw.
	PacketContainer GetPacketToRecv();

	// Transfers formatted outgoing data into the security object. A packet
	// is then queued internally and will be processed when the GetPacketToSend
	// function is called. This function is very lightweight, so no heavy processing
	// is done.
	void Send( uint16_t opcode, const StreamUtility & data, uint8_t encrypted = false, uint8_t massive = false );
	void Send( uint16_t opcode, const uint8_t * data, int32_t count, uint8_t encrypted = false, uint8_t massive = false );

	// Returns true if there are any packets ready to be sent. This function should
	// be called regularly after data is sent to check to see if there is more data 
	// waiting to be sent still.
	uint8_t HasPacketToSend() const;

	// Returns the raw packet data to send, which has been properly formated to 
	// the Silkroad protocol. This is a very heavy function that should only be 
	// called within a serialized network thread. Can throw.
	std::vector< uint8_t > GetPacketToSend();

	// When the security mode is set to include security bytes, certain opcodes 
	// must be added to comply with the security system. The security system will
	// pre-add the GatewayServer packet opcodes as needed. The user should add the
	// * item use opcode * for the current version of Silkroad they are using.
	void AddEncryptedOpcode( uint16_t opcode );

	// Returns if the current client, has finished handshake process.
	bool IsFinished();
};

//-----------------------------------------------------------------------------

#endif
