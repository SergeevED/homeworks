#ifndef IRC_CLIENT_NEWSRECEIVER_H
#define IRC_CLIENT_NEWSRECEIVER_H

#include <iostream>
#include <string>
#include <algorithm>
#include <ctime>
#include <curl/curl.h>


class NewsReceiver {
public:
    NewsReceiver();
    ~NewsReceiver();
    void getRandomNews(std::string& news);

private:
    // TODO: add smart pointer
    CURL *mCurl;
    const std::string mUrl = "https://newsapi.org/v1/articles?source=google-news&sortBy=top&"
            "apiKey=39d5e77100d64fe3a271e941f3ef8f63";
};

#endif //IRC_CLIENT_NEWSRECEIVER_H
