CC=gcc
INCLUDES=-I../ThirdParty/voro++-0.4.6/src
CFLAGS=-O3 -Wall -std=c++17
DEPS = ../VoroGen/VoroGen2.cc
OBJ = VoroGen2.o 

MAC_ROOT=$(xcrun --sdk macosx --show-sdk-path)
MAC_ARGS=-target arm64-apple-macos14 --sysroot $MAC_ROOT -isysroot $MAC_ROOT -fPIC
IOS_ROOT=$(xcrun --sdk iphoneos --show-sdk-path)
IOS_ARGS=-target arm64-apple-ios --sysroot $IOS_ROOT -isysroot $IOS_ROOT -fembed-bitcode

%.o: %.cc $(DEPS)
	$(CC) -c -o $@ $< $(CFLAGS) $(INCLUDES) $(MAC_ARGS) 

hellomake: $(OBJ)
	$(CC) -o $@ $^ $(CFLAGS)
