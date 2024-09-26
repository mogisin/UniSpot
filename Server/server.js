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
        console.log(monsters)
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
    
        // 현재 위치 근처의 몬스터 찾기
        const allMonsters = await Monster.find({ captured: false });
        
        // 반경 10미터 이내의 몬스터 필터링
        const nearbyMonsters = allMonsters.filter(monster => {
            const distance = getDistanceFromLatLonInMeters(
                latitude,
                longitude,
                monster.location.latitude,
                monster.location.longitude
            );
            return distance <= 10; // 10미터 이내인지 확인
        });
    
        console.log('반경 10미터 이내의 주변 몬스터:', nearbyMonsters);

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

// 위도 1도는 약 111,320미터, 경도 1도는 위도에 따라 변화하므로 약 111,320 * cos(latitude)
function getRandomCoordinateWithinRadius(latitude, longitude, radiusInMeters = 30) {
  const earthRadiusInMeters = 6371000; // 지구 반지름 (미터)
  
  // 랜덤 거리와 방향 설정
  const randomDistance = Math.random() * radiusInMeters;
  const randomAngle = Math.random() * 2 * Math.PI;

  // 위도 변화량 계산 (거리 / 지구 반지름)
  const deltaLatitude = (randomDistance / earthRadiusInMeters) * (180 / Math.PI);
  // 경도 변화량 계산 (거리 / 지구 반지름 * cos(위도))
  const deltaLongitude = (randomDistance / (earthRadiusInMeters * Math.cos(latitude * Math.PI / 180))) * (180 / Math.PI);

  // 랜덤하게 위도 경도에 추가
  const newLatitude = latitude + deltaLatitude * Math.cos(randomAngle);
  const newLongitude = longitude + deltaLongitude * Math.sin(randomAngle);

  return { newLatitude, newLongitude };
}


// Haversine 공식을 이용해 두 좌표 간의 거리를 계산하는 함수
function getDistanceFromLatLonInMeters(lat1, lon1, lat2, lon2) {
  const R = 6371000; // 지구 반경 (미터)
  const dLat = deg2rad(lat2 - lat1);  // 경도 차이
  const dLon = deg2rad(lon2 - lon1);  // 위도 차이
  const a = 
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
      Math.sin(dLon / 2) * Math.sin(dLon / 2)
  ; 
  const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a)); 
  const distance = R * c; // 미터 단위 거리
  return distance;
}

// 도(degrees)를 라디안(radians)으로 변환하는 함수
function deg2rad(deg) {
  return deg * (Math.PI / 180);
}

// 랜덤 몬스터 생성 함수
async function generateRandomMonsters(count = 5) {
    const monsters = [];

    const departmentCoordinates = {
      '소프트웨어융합대학': { latitude: 37.239782, longitude: 127.083313 },
      '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
      '응용과학대학': { latitude: 37.239782, longitude: 127.083313 },
      '체육대학': { latitude: 37.244493, longitude: 127.080436 },
      '공과대학': { latitude: 37.246468, longitude: 127.080844 },
      '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 }
    };

    const departments = Object.keys(departmentCoordinates);

    for (let i = 0; i < count; i++) {
    //   const randomLatitude = (Math.random() * 180 - 90).toFixed(6);
    //   const randomLongitude = (Math.random() * 360 - 180).toFixed(6);
      
      // const randomLatitude = 37.243703; //경희대 중앙 위도
      // const randomLongitude = 127.080085; //경희대 중앙 경도 

      
      // 랜덤 학과 선택
      const randomDept = departments[Math.floor(Math.random() * departments.length)];
      const { latitude, longitude } = departmentCoordinates[randomDept];
      const { newLatitude, newLongitude } = getRandomCoordinateWithinRadius(latitude, longitude);

      const monster = new Monster({
        name: `Monster_${Math.floor(Math.random() * 1000)}`,
        dept: randomDept, // 랜덤 학과 설정
        description: '몬스터 설명',
        tags:['태그1','태그2'],
        location: {
          latitude: newLatitude,
          longitude: newLongitude
        },
        captured: false
      });
      
      await monster.save(); // MongoDB에 저장
      monsters.push(monster);
    }
    
    return monsters;
  }
  
