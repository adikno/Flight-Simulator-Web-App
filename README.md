# Flight-Simulator-Web-App
A restful-web application, supports several activities, all related to displaying the route of a "Flight-Gear" simulator.
The application connects to the simulator as a client, in order to gets the latitude and longitude of the plane at real time.

- **/display/127.0.0.1/5400 – 
display the current location of the simulator in IP 127.0.0.1 on port 5400

- **/display/127.0.0.1/5400/4 –
display the simulator's route on the map, refresh the display every 4 seconds.

- **/save/127.0.0.1/5400/4/10/flight1 –
sample the simulator's location 4 time per seconds for 10 seconds and save the data in a file calles 'flight1' in order to restore the route later.

- **display/flight1/4 
– load the data from a file called 'flight1' and display an animation of the route on the map.
