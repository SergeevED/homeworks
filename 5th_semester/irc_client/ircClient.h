#ifndef IRC_CLIENT_IRC_CLIENT_H
#define IRC_CLIENT_IRC_CLIENT_H

#include <unistd.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <cstring>

#include <iostream>
#include <algorithm>
#include <string>
#include <stdexcept>
#include <thread>
#include <mutex>
#include <chrono>


class ircClient {
public:
    ircClient(std::string host, uint16_t port);
    ~ircClient();
    void start(std::string nick, std::string channel);
    bool sendMessage(const char *msg) const;
    ssize_t receiveMessage(char *msg) const;

private:
    int mDescriptor = -1;
    std::thread mSenderThread;
    const int64_t mSleepTime = 50;
    static constexpr size_t mMaxMessageLength = 512;
    const char* mTrailingCharacters = "\r\n";
};


#endif //IRC_CLIENT_IRC_CLIENT_H
