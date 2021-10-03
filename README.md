# OpenTelemetry Demo Project

Run docker: `docker-compose -f docker-compose.yml up -d`  
Remove docker: `docker rm $(docker ps -a -q) -f`

  
Jeager URL: localhost:16686  
Grafana: 
~~~
http://localhost:3000/   
User: admin  
Password: password1
~~~
Graylog: Web Interface (http://localhost:9000/)  

Create Input (GELF UDP):
![image](https://user-images.githubusercontent.com/1341095/135771363-1f86bdbe-9104-4c26-88ce-cefd5030ffdb.png)
![image](https://user-images.githubusercontent.com/1341095/135771426-608c640c-ecfc-4c65-aaf4-f50c638ab344.png)
