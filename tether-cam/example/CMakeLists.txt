cmake_minimum_required (VERSION 2.6)

link_libraries(apriltags)

add_executable(test549 test549.cpp TCPStream.cpp TCPAcceptor.cpp)
pods_install_executables(test549)
