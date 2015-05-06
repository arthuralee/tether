import serial
import socket
import sys

sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('localhost', 8081)
print ('starting up on %s port %s' % server_address)
sock.bind(server_address)

# Listen for incoming connections
sock.listen(1)

while True:
    # Wait for a connection
    print >>sys.stderr, 'waiting for a connection'
    connection, client_address = sock.accept()

    try:
        print >>sys.stderr, 'connection from', client_address

        ser = serial.Serial('/dev/tty.usbserial-FTHFFDCT', 115200)
        ser.flushInput()
        ser.flushOutput()

        while True:
            line = ser.readline()
            if len(line) > 1:
                connection.sendall(line)
    except socket.error, e:
        ser.close()
        connection.close()
    except IOError, e:
        ser.close()
        connection.close()
    finally:
        # Clean up the connection
        ser.close()
        connection.close()
