cmake_minimum_required(VERSION 3.6)
project(irc_client)

set(CMAKE_CXX_FLAGS ${CMAKE_CXX_FLAGS} "-std=c++11 -pthread")

find_package (Threads)

set(SOURCE_FILES main.cpp IrcClient.cpp IrcClient.h NewsReceiver.cpp NewsReceiver.h)
add_executable(irc_client ${SOURCE_FILES})
target_link_libraries (irc_client ${CMAKE_THREAD_LIBS_INIT})
target_link_libraries (irc_client curl)