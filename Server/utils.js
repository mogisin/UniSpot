const User = require('./models/User');
const Monster = require('./models/Monster');

// Haversine 공식을 이용해 두 좌표 간의 거리를 계산하는 함수
function getDistanceFromLatLonInMeters(lat1, lon1, lat2, lon2) {
    const R = 6371000; // 지구 반경 (미터)
    const dLat = deg2rad(lat2 - lat1);  // 경도 차이
    const dLon = deg2rad(lon2 - lon1);  // 위도 차이
    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
      Math.sin(dLon / 2) * Math.sin(dLon / 2);
    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    const distance = R * c; // 미터 단위 거리
    return distance;
  }
  
  // 도(degrees)를 라디안(radians)으로 변환하는 함수
  function deg2rad(deg) {
    return deg * (Math.PI / 180);
  }
  
  // 랜덤 좌표 생성
  function getRandomCoordinateWithinRadius(latitude, longitude, radiusInMeters = 50) {
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
  
  // 랜덤 몬스터 생성 함수
  async function generateRandomMonsters(count = 5) {
    const monsters = [];
  
    const departmentCoordinates = {
      '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
      '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
      '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
      '체육대학': { latitude: 37.244493, longitude: 127.080436 },
      '공과대학': { latitude: 37.246468, longitude: 127.080844 },
      '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
      '외국어대학':{latitude:37.245391, longitude:127.077649},
      '생명과학대학':{latitude:37.242962,longitude:127.080932}
    };
  
    const departments = Object.keys(departmentCoordinates);
  
    for (let i = 0; i < count; i++) {
      const randomDept = departments[Math.floor(Math.random() * departments.length)];
      const { latitude, longitude } = departmentCoordinates[randomDept];
      const { newLatitude, newLongitude } = getRandomCoordinateWithinRadius(latitude, longitude);
  
      const monster = new Monster({
        name: `Monster_${Math.floor(Math.random() * 1000)}`,
        dept: randomDept,
        description: '몬스터 설명',
        tags: ['태그1', '태그2'],
        location: {
          latitude: newLatitude,
          longitude: newLongitude
        },
        captured: false
      });
  
      await monster.save();
      monsters.push(monster);
    }
  
    return monsters;
  }
  
  module.exports = {
    getDistanceFromLatLonInMeters,
    getRandomCoordinateWithinRadius,
    generateRandomMonsters
  };
  