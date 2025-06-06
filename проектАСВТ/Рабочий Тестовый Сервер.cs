#include <iostream>
#include <string>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <vector>
#include <sstream>
#include <iomanip>
#include <algorithm>

#define PORT 1234
#define BUFFER_SIZE 1024

using namespace std;

float criticalTemp = 100.0;
float criticalMQ135 = 250.0;
float criticalHumidity = 700.0;
const string XOR_KEY = "asdfqwer";

vector<string> split(const string &s, char delimiter) {
    vector<string> tokens;
    string token;
    istringstream tokenStream(s);
    while (getline(tokenStream, token, delimiter)) {
        tokens.push_back(token);
    }
    return tokens;
}

string hexToString(const string& hex) {
    string result;
    for (size_t i = 0; i < hex.length(); i += 2) {
        string byteString = hex.substr(i, 2);
        char byte = (char) strtol(byteString.c_str(), nullptr, 16);
        result += byte;
    }
    return result;
}

string xorDecrypt(const string& input, const string& key) {
    string output;
    for (size_t i = 0; i < input.size(); ++i) {
        output += input[i] ^ key[i % key.size()];
    }
    return output;
}

void handleClient(int client_socket) {
    char buffer[BUFFER_SIZE] = {0};
    int bytes_read = read(client_socket, buffer, BUFFER_SIZE - 1);

    if (bytes_read > 0) {
        string request(buffer, bytes_read);

        // Удаляем символы новой строки и возврата каретки
        request.erase(remove(request.begin(), request.end(), '\r'), request.end());
        request.erase(remove(request.begin(), request.end(), '\n'), request.end());

        cout << "Received: '" << request << "'" << endl;

        if (request == "GET_CRITICAL") {
            string response = to_string(criticalTemp) + "," +
                              to_string(criticalMQ135) + "," +
                              to_string(criticalHumidity) + "\n";
            send(client_socket, response.c_str(), response.length(), 0);
            cout << "Sent critical values: " << response;
        }
        else if (request == "CHECK_REQUEST") {
            string response = "ACK\n";
            send(client_socket, response.c_str(), response.length(), 0);
            cout << "Sent ACK" << endl;
        }
        else {
            try {
                // Пытаемся расшифровать как HEX-данные
                string hexData = request;
                string bytes = hexToString(hexData);
                string decrypted = xorDecrypt(bytes, XOR_KEY);

                cout << "Decrypted data: " << decrypted << endl;

                if (decrypted.find("data:") != string::npos) {
                    // Обработка данных с датчиков
                    vector<string> values = split(decrypted.substr(5), ',');
                    if (values.size() >= 6) {
                        cout << "MQ-135: " << values[0] << endl;
                        cout << "Humidity: " << values[1] << "%" << endl;
                        cout << "Temperature: " << values[2] << "°C" << endl;
                        cout << "Alerts - Temp: " << (values[3] == "1" ? "YES" : "NO");
                        cout << ", MQ135: " << (values[4] == "1" ? "YES" : "NO");
                        cout << ", Hum: " << (values[5] == "1" ? "YES" : "NO") << endl;
                    }

                    string response = "DATA_RECEIVED\n";
                    send(client_socket, response.c_str(), response.length(), 0);
                } else {
                    cout << "Unknown decrypted request: " << decrypted << endl;
                    string response = "ERROR:UNKNOWN_REQUEST\n";
                    send(client_socket, response.c_str(), response.length(), 0);
                }
            } catch (...) {
                cout << "Error processing encrypted data" << endl;
                string response = "ERROR:INVALID_DATA_FORMAT\n";
                send(client_socket, response.c_str(), response.length(), 0);
            }
        }
    } else {
        cout << "Error reading from socket or connection closed" << endl;
    }

    close(client_socket);
    cout << "Connection closed" << endl << endl;
}

int main() {
    int server_fd, new_socket;
    struct sockaddr_in address;
    int opt = 1;
    int addrlen = sizeof(address);

    if ((server_fd = socket(AF_INET, SOCK_STREAM, 0)) == 0) {
        perror("socket failed");
        exit(EXIT_FAILURE);
    }

    if (setsockopt(server_fd, SOL_SOCKET, SO_REUSEADDR, &opt, sizeof(opt))) {
        perror("setsockopt");
        exit(EXIT_FAILURE);
    }

    address.sin_family = AF_INET;
    address.sin_addr.s_addr = INADDR_ANY;
    address.sin_port = htons(PORT);

    if (bind(server_fd, (struct sockaddr *)&address, sizeof(address)) < 0) {
        perror("bind failed");
        exit(EXIT_FAILURE);
    }

    if (listen(server_fd, 3) < 0) {
        perror("listen");
        exit(EXIT_FAILURE);
    }

    cout << "Server listening on port " << PORT << endl;

    while (true) {
        cout << "Waiting for connection..." << endl;
        if ((new_socket = accept(server_fd, (struct sockaddr *)&address, (socklen_t*)&addrlen)) < 0) {
            perror("accept");
            continue;
        }

        cout << "Connection from " << inet_ntoa(address.sin_addr) << ":" << ntohs(address.sin_port) << endl;
        handleClient(new_socket);
    }

    return 0;
}
