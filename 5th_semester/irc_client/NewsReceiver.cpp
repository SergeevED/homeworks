#include "NewsReceiver.h"


NewsReceiver::NewsReceiver() {
    curl_global_init(CURL_GLOBAL_DEFAULT);
    mCurl = curl_easy_init();
}

NewsReceiver::~NewsReceiver() {
    curl_global_cleanup();
}

static size_t writeCallback(void *contents, size_t size, size_t nmemb, void *userp)
{
    ((std::string*)userp)->append((char*)contents, size * nmemb);
    return size * nmemb;
}

void NewsReceiver::getRandomNews(std::string& news) {
    if (mCurl) {
        CURLcode res;

        /*auto writeCallback = [] (void *contents, size_t size, size_t nmemb, void *userp) {
            ((std::string*)userp)->append((char*)contents, size * nmemb);
            return size * nmemb;
        };*/
        std::string allNews = std::string();

        curl_easy_setopt(mCurl, CURLOPT_URL, mUrl.c_str());
        curl_easy_setopt(mCurl, CURLOPT_WRITEFUNCTION, writeCallback);
        curl_easy_setopt(mCurl, CURLOPT_WRITEDATA, &allNews);
        curl_easy_setopt(mCurl, CURLOPT_SSL_VERIFYHOST, 0L);
        // Perform the request, res will get the return code
        res = curl_easy_perform(mCurl);
        // Check for errors
        if (res != CURLE_OK)
            fprintf(stderr, "curl_easy_perform() failed: %s\n",
                    curl_easy_strerror(res));
        curl_easy_cleanup(mCurl);

        long newsAmount = std::count(allNews.begin(), allNews.end(), '_') - 1;
        long newsNumber = (unsigned(std::time(0))) % (newsAmount - 1) + 1;

        unsigned long index = 1;
        for (int i = 0; i < newsNumber; i++) {
            index = allNews.find('{', index) + 1;
        }

        std::string titleSuffix("\"title\":\"");
        unsigned long titleIndex = allNews.find(titleSuffix, index) + titleSuffix.length();
        std::string title = allNews.substr(titleIndex, allNews.find('"', titleIndex) - titleIndex);

        std::string urlSuffix("\"url\":\"");
        unsigned long urlIndex = allNews.find(urlSuffix, index) + urlSuffix.length();
        std::string url = allNews.substr(urlIndex, allNews.find('"', urlIndex) - urlIndex);

        news.append(title + "   " + url);
    }
}
