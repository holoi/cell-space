#!/bin/sh

CFLAGS="-O3 -Wall -std=c++17 "
CC=gcc
AR=ar

INCLUDES="-I../ThirdParty/voro++-0.4.6/src"
SOURCES="../ThirdParty/voro++-0.4.6/src"

rm -rf *.o *.so *.a *.bundle *.dylib 
set -x

## MacOS 
# arm64
MAC_ROOT=`xcrun --sdk macosx --show-sdk-path`
ARCH_TARGET="-target arm64-apple-macos"
MAC_ARGS="$ARCH_TARGET --sysroot $MAC_ROOT -isysroot $MAC_ROOT -fPIC"

$CC $CFLAGS $INCLUDES $MAC_ARGS -c ../VoroGen/VoroGen.cc -o VoroGen.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/voro++.cc -o voro++.o 

$CC $MAC_ARGS -fPIC -rdynamic -shared -o libVoroGen_arm64.dylib VoroGen.o voro++.o -lstdc++ 

## x86_64
MAC_ROOT=`xcrun --sdk macosx --show-sdk-path`
ARCH_TARGET="-target x86_64-apple-macos"
MAC_ARGS="$ARCH_TARGET --sysroot $MAC_ROOT -isysroot $MAC_ROOT -fPIC"

$CC $CFLAGS $INCLUDES $MAC_ARGS -c ../VoroGen/VoroGen.cc -o VoroGen.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/voro++.cc -o voro++.o 

$CC $MAC_ARGS -fPIC -rdynamic -shared -o libVoroGen_x86_64.dylib VoroGen.o voro++.o -lstdc++ 

lipo -create -output VoroGen.bundle libVoroGen_arm64.dylib libVoroGen_x86_64.dylib

DST="../../../Runtime/macOS"
rm -rf $DST/VoroGen.bundle
cp -r VoroGen.bundle $DST

# iOS  
IOS_ROOT=`xcrun --sdk iphoneos --show-sdk-path`
ARCH_TARGET="-target arm64-apple-ios"
IOS_ARGS="--sysroot $IOS_ROOT -isysroot $IOS_ROOT -fembed-bitcode"

$CC $CFLAGS $INCLUDES $IOS_ARGS -c ../VoroGen/VoroGen.cc -o VoroGen.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/voro++.cc -o voro++.o 

$AR -crv libVoroGen.a VoroGen.o voro++.o 

DST="../../../Runtime/iOS"
cp libVoroGen.a $DST

#rm -rf *.o *.so *.a *.bundle *.dylib 
