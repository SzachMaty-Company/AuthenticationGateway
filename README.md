# AuthenticationGateway
Base endpoint: http://localhost:5000
Application edpoints:   
    - chat service:
        - /registerws
        - /registerws/info
        - /topic/chat (websocket)
        - /app/chat (websocket)
    - user data service:
        - /auth/google
        - /protected (requires auth)