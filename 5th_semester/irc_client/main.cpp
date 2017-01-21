#include "IrcClient.h"


int main() {
    try {
        std::string nick;
        std::cout << "Enter your nickname:\n";
        std::cin >> nick;
        IrcClient ic(std::string("irc.freenode.org"), 6667);
        ic.start(nick, std::string("#spbtesting"));
    } catch (const std::runtime_error& error) {
        std::cout << error.what();
    }

    return 0;
}
