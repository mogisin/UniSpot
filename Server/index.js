const mongoose = require('mongoose');
require('dotenv').config();

// MongoDB 연결
const mongoURI = process.env.MONGODB_URI;
mongoose.connect(mongoURI)
  .then(() => console.log('MongoDB connected'))
  .catch(err => console.error('MongoDB connection error:', err));

// WebSocket 서버 실행
require('./websocket');
