import socket
import threading

class UdpChat:
    def __init__(self, ip:str, port:int):
        self.ip = ip
        self.port = port
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.bind((ip, port))
        self.sock.settimeout(0.2)
        self.running = True
        print(f"Server started on port {self.sock.getsockname()[0]}:{self.port}")

    def send_to(self, message:str, addr:tuple=(str, int)):
        self.sock.sendto(message.encode(), addr)
    
    def broadcast(self, message:str):
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
        self.sock.sendto(f"broadcast: {message}".encode(), ("<broadcast>", self.port))
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 0)
 
    def start_receive_loop(self):
        self.receive_thread = threading.Thread(target=self.receive)
        self.receive_thread.daemon = True
        self.receive_thread.start()

    def receive(self):
        while self.running:
            try:
                data, addr = self.sock.recvfrom(1024)
                print(f"Received message from {addr}: {data.decode()}")
            except socket.timeout:
                pass

    def close(self):
        self.running = False
        if self.receive_thread.is_alive():
            self.receive_thread.join()
        self.sock.close()

    def chat_input(self):
        print("Input format is '<ip>:<port> <message>' to send a message.\n/broadcast <message> to broadcast a message to all clients.\n")
        print("Type 'exit' to stop the server")

        while self.running:
            message = input()
            if message == "exit":
                self.close()
            else:
                try:
                    if message.startswith("/broadcast"):
                        message = message.split(" ", 1)[1]
                        self.broadcast(message)
                    else:
                        addr, message = message.split(" ", 1)
                        ip, port = addr.split(":")
                        self.send_to(message, (ip, int(port)))
                except Exception as e:
                    print(f"Invalid input:\nInput format is '<ip>:<port> <message>' to send a message\n/broadcast <message> to broadcast a message to all clients\n")
        chat.close()

    def start_chat(self):
        self.start_receive_loop()
        self.chat_input()

if __name__ == "__main__":
    port = int(input("Enter the port: "))
    ip = input("Enter the IP address: ")
    chat = UdpChat(ip, port)
    chat.start_chat()