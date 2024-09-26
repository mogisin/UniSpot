const WebSocket = require('ws');
const { handleMessage } = require('./messageHandler');

const wss = new WebSocket.Server({ port: 8000 });

wss.on('connection', (ws) => {
  console.log('Client connected');

  // 클라이언트에서 메시지 수신
  ws.on('message', async (message) => {
    await handleMessage(ws, message); // 메시지 처리 함수 호출
  });

  ws.on('close', () => {
    console.log('Client disconnected');
  });
});

console.log('WebSocket server running on ws://localhost:8000');
