#!/bin/sh

CFLAGS="-O3 -Wall -std=c++17 "
CC=gcc
AR=ar

INCLUDES="-I../ThirdParty/voro++-0.4.6/src"
SOURCES="../ThirdParty/voro++-0.4.6/src"
VORO_OBJS="cell.o common.o container.o unitcell.o v_compute.o c_loops.o v_base.o wall.o pre_container.o container_prd.o "

rm -rf *.o *.so *.a *.bundle *.dylib 
set -x

## MacOS 
MAC_ROOT=`xcrun --sdk macosx --show-sdk-path`
MAC_ARGS="--sysroot $MAC_ROOT -isysroot $MAC_ROOT -fPIC"

$CC $CFLAGS $INCLUDES $MAC_ARGS -c ../VoroGen/VoroGen.cc -o VoroGen.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/cell.cc -o cell.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/common.cc -o common.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/container.cc -o container.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/unitcell.cc -o unitcell.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/v_compute.cc -o v_compute.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/c_loops.cc -o c_loops.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/v_base.cc -o v_base.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/wall.cc -o wall.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/pre_container.cc -o pre_container.o 
$CC $CFLAGS $INCLUDES $MAC_ARGS -c ${SOURCES}/container_prd.cc -o container_prd.o 

$CC -fPIC -rdynamic -shared -o libVoroGen.dylib VoroGen.o $VORO_OBJS -lstdc++ 
lipo -create -output VoroGen.bundle libVoroGen.dylib

DST="../../../Runtime/macOS"
rm -rf $DST/VoroGen.bundle
cp -r VoroGen.bundle $DST

# # iOS  
IOS_ROOT=`xcrun --sdk iphoneos --show-sdk-path`
IOS_ARGS="-target arm64-apple-ios --sysroot $IOS_ROOT -isysroot $IOS_ROOT -fembed-bitcode"

$CC $CFLAGS $INCLUDES $IOS_ARGS -c ../VoroGen/VoroGen.cc -o VoroGen.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/cell.cc -o cell.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/common.cc -o common.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/container.cc -o container.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/unitcell.cc -o unitcell.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/v_compute.cc -o v_compute.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/c_loops.cc -o c_loops.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/v_base.cc -o v_base.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/wall.cc -o wall.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/pre_container.cc -o pre_container.o 
$CC $CFLAGS $INCLUDES $IOS_ARGS -c ${SOURCES}/container_prd.cc -o container_prd.o 

$AR -crv libVoroGen.a VoroGen.o $VORO_OBJS

DST="../../../Runtime/iOS"
cp libVoroGen.a $DST

rm -rf *.o *.so *.a *.bundle *.dylib 
