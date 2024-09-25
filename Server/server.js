const WebSocket = require('ws');
const mongoose = require('mongoose');
const User = require('./models/User');
const Monster = require('./models/Monster');

require('dotenv').config();
const mongoURI = process.env.MONGODB_URI;

// MongoDB 연결
mongoose.connect(mongoURI)
  .then(() => console.log('MongoDB connected'))
  .catch(err => console.error('MongoDB connection error:', err));

const wss = new WebSocket.Server({ port: 8000 });

wss.on('connection', (ws) => {
  console.log('Client connected');

  // 클라이언트에서 메시지 수신
  ws.on('message', async (message) => {
    try {
      const data = JSON.parse(message);
    //   console.log('Received message:', data);

      // 랜덤 몬스터 더미 데이터 생성
      if (data.type === 'generate_monsters') {
        const monsters = await generateRandomMonsters(data.count || 5);
        ws.send(JSON.stringify({
          type: 'generate_monsters_success',
          message: `${monsters.length} monsters created successfully.`,
          monsters: monsters
        }));
        console.log(`${monsters.length} monsters generated and stored in DB.`);
      }

      // 위치 정보 처리
      else if (data.type === 'location') {
        const { username, latitude, longitude } = data;

        // 유저의 위치 정보 업데이트
        let user = await User.findOne({ username });
        if (!user) {
          user = new User({ username, location: { latitude, longitude } });
          console.log('새로운 유저 생성:', user);
        } else {
          user.location.latitude = latitude;
          user.location.longitude = longitude;
          console.log('유저 위치 업데이트:', user);
        }
        await user.save();
        console.log('유저 저장 완료:', user);

        // 현재 위치 주변의 몬스터 찾기
        const nearbyMonsters = await Monster.find({
          'location.latitude': { $gte: latitude - 0.01, $lte: latitude + 0.01 },
          'location.longitude': { $gte: longitude - 0.01, $lte: longitude + 0.01 },
          captured: false
        });
        console.log('주변 몬스터:', nearbyMonsters);

        // 클라이언트로 몬스터 정보 전송
        ws.send(JSON.stringify({
          type: 'nearby_monsters',
          monsters: nearbyMonsters
        }));

      } else if (data.type === 'capture') {
        const { username, monsterId } = data;

        // 몬스터 포획 처리
        const monster = await Monster.findById(monsterId);
        if (monster && !monster.captured) {
          monster.captured = true;
          await monster.save();
          console.log('몬스터 포획 완료:', monster);

          // 유저의 몬스터 목록에 추가
          let user = await User.findOne({ username });
          user.monsters.push({ name: monster.name, capturedAt: new Date() });
          await user.save();
          console.log('유저 몬스터 목록 업데이트:', user);

          // 몬스터 포획 성공 메시지 전송
          ws.send(JSON.stringify({
            type: 'capture_success',
            monster: monster.name
          }));
        }
      }
    } catch (error) {
      console.error('Error processing message:', error);
    }
  });

  ws.on('close', () => {
    console.log('Client disconnected');
  });
});

console.log('WebSocket server running on ws://localhost:8000');

// 랜덤 몬스터 생성 함수
async function generateRandomMonsters(count = 5) {
    const monsters = [];
    const departments = ['소프트웨어융합대학', '공과대학', '전자정보대학', '예술디자인대학','외국어대학']; // 학과 목록
    
    for (let i = 0; i < count; i++) {
    //   const randomLatitude = (Math.random() * 180 - 90).toFixed(6);
    //   const randomLongitude = (Math.random() * 360 - 180).toFixed(6);
      
      const randomLatitude = 37.7749;
      const randomLongitude = -122.4194;

      
      // 랜덤 학과 선택
      const randomDept = departments[Math.floor(Math.random() * departments.length)];
      
      const monster = new Monster({
        name: `Monster_${Math.floor(Math.random() * 1000)}`,
        dept: randomDept, // 랜덤 학과 설정
        description: '몬스터 설명',
        tags:['태그1','태그2'],
        location: {
          latitude: randomLatitude,
          longitude: randomLongitude
        },
        captured: false
      });
      
      await monster.save(); // MongoDB에 저장
      monsters.push(monster);
    }
    
    return monsters;
  }
  
