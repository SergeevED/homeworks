#include "ircClient.h"


ircClient::ircClient(std::string host, uint16_t port) {
    hostent *ircHostent = gethostbyname(host.c_str());
    if (ircHostent == NULL) {
        throw std::runtime_error("Cannot resolve host name: " + host);
    }

    mDescriptor = socket(PF_INET, SOCK_STREAM, 0);
    if (mDescriptor < 0) {
        throw std::runtime_error("Cannot open socket");
    }

    sockaddr_in ircSockaddr;
    ircSockaddr.sin_family = AF_INET;
    ircSockaddr.sin_port = htons(port);
    ircSockaddr.sin_addr = *((in_addr*)ircHostent->h_addr);
    std::fill(ircSockaddr.sin_zero, ircSockaddr.sin_zero + sizeof(ircSockaddr.sin_zero), '\0');

    if (connect(mDescriptor, (sockaddr*)&ircSockaddr, sizeof(sockaddr)) < 0) {
        throw std::runtime_error("Cannot establish connection with " + host);
    }

    auto sender = [this]() {
        char buffer[mMaxMessageLength + 1];
        do {
            std::this_thread::sleep_for(std::chrono::milliseconds(mSleepTime));
            const char *suffix = "PRIVMSG #spbtesting :";
            size_t suffixLength = strlen(suffix);
            strcpy(buffer, suffix);
            std::cin.getline(buffer + suffixLength, mMaxMessageLength - strlen(mTrailingCharacters) - suffixLength);
        } while (sendMessage(buffer));
        std::cout << "Error: cannot send message!\n";
    };
    mSenderThread = std::thread(sender);
}

ircClient::~ircClient() {
    sendMessage("QUIT");
    close(mDescriptor);
}

void ircClient::start(std::string nick, std::string channel) {
    sendMessage(("USER " + nick + " * * : ").c_str());
    sendMessage(("NICK " + nick).c_str());
    sendMessage(("JOIN " + channel).c_str());

    while (true) {
        char buffer[mMaxMessageLength + 1] = {'\0'};
        ssize_t bytesReceived = receiveMessage(buffer);
        if (bytesReceived <= 0) {
            throw std::runtime_error("Failed to read new message from server");
        }

        std::string msg(buffer);

        if (msg.find("PING") == 0) {
            sendMessage((std::string("PO") + (msg.c_str() + 2)).c_str());
        } else if (msg[0] == ':') {
            msg.erase(0, 1);
            const size_t firstSpaceIndex = msg.find(' ');
            if (firstSpaceIndex + 1 == msg.find("PART")) {
                std::cout << msg.erase(msg.find('!'),  msg.find("PART") - msg.find('!') - 1);
            } else if (firstSpaceIndex + 1 == msg.find("JOIN")) {
                std::cout << msg.erase(msg.find('!'),  msg.find("JOIN") - msg.find('!') - 1);
            } else if (firstSpaceIndex + 1 == msg.find("PRIVMSG")) {
                std::cout << msg.erase(msg.find('!'),  msg.find('#') - msg.find('!') - 1);
            }
        }
        std::cout << msg;

        std::this_thread::sleep_for(std::chrono::milliseconds(mSleepTime));
    }
}

bool ircClient::sendMessage(const char *msg) const {
    const size_t msgLength = strlen(msg) + strlen(mTrailingCharacters);
    char buffer[mMaxMessageLength + 1] = {'\0'};
    ssize_t bytesSent = write(mDescriptor, strcat(strcat(buffer, msg), mTrailingCharacters), msgLength);
    return (bytesSent >= 0 && (size_t)bytesSent == msgLength);
}

ssize_t ircClient::receiveMessage(char *msg) const {
    return read(mDescriptor, msg, mMaxMessageLength);
}
