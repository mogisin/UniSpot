const User = require('./models/User');
const Monster = require('./models/Monster');
const Spot = require('./models/Spot');

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
  
  async function newgenerateRandomMonsters(count, name = undefined) { //중앙도서관 및 위치정보를 db에서 받아오도록 수정됨
    const monsters = [];
    
    // DB에서 Spot 컬렉션에서 좌표 정보를 불러옴
    const spots = await Spot.find({});

    // Spot 데이터를 {name: {latitude, longitude}} 형식으로 변환
    const departmentCoordinates = {};
    spots.forEach(spot => {
        departmentCoordinates[spot.spotName] = {
            latitude: spot.location.latitude,
            longitude: spot.location.longitude
        };
    });

    const departments = Object.keys(departmentCoordinates);

    const departmentAbbreviations = {
      '소프트웨어융합대학': '소융대',
      '전자정보대학': '전정대',
      '응용과학대학': '응과대',
      '체육대학': '체대',
      '공과대학': '공대',
      '예술디자인대학': '예디대',
      '외국어대학': '외대',
      '생명과학대학': '생대',
      '국제대학': '국제대',
      '중앙도서관': '중앙도서관'
    };
    const departmentAbbreviations2 = {
      'Software': '소프트웨어융합대학',
      'Electronics&Information': '전자정보대학',
      'Applied_Sciences': '응용과학대학',
      'Physical_Education': '체육대학',
      'Engineering': '공과대학',
      'Art&Design': '예술디자인대학',
      'Foreign_Language': '외국어대학',
      'Life_Sciences': '생명과학대학',
      'International': '국제대학'
  };
    if (name === undefined){
      console.log('몬스터 이름 없음');
      for (let i = 0; i < count; i++) {
        let randomDept = departments[Math.floor(Math.random() * departments.length)];
        let latitude, longitude;
        let generateSpot;
  
        if (randomDept === '중앙도서관') {
          // '중앙도서관'일 경우 다른 학과를 랜덤 선택 (중앙도서관 제외)
          const otherDepartments = departments.filter(dept => dept !== '중앙도서관');
          randomDept = otherDepartments[Math.floor(Math.random() * otherDepartments.length)];
  
          // 위치는 중앙도서관의 좌표를 사용
          const centralCoordinates = departmentCoordinates['중앙도서관'];
          latitude = centralCoordinates.latitude;
          longitude = centralCoordinates.longitude;
  
          generateSpot = '중앙도서관';
          
          
        } else {
          // 일반적인 경우 해당 학과의 좌표를 사용
          const deptCoordinates = departmentCoordinates[randomDept];
          latitude = deptCoordinates.latitude;
          longitude = deptCoordinates.longitude;
          generateSpot = randomDept;
        }
  
        // 좌표 범위 내에서 새로운 좌표 생성
        const { newLatitude, newLongitude } = getRandomCoordinateWithinRadius(latitude, longitude);
  
        const monster = new Monster({
          name: `${departmentAbbreviations[randomDept]}몬_${Math.floor(Math.random() * 1000)}`,
          dept: randomDept,
          generateSpot: generateSpot,
          description: `${departmentAbbreviations[generateSpot]}에 주로 출몰함`,
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
    } else {
      // 숫자 제외한 이름 추출 (예: Applied_Sciences, ArtDesign 등)
      // 숫자와 (Clone) 제거하여 영어 단과대 이름만 추출
      const departmentKey = name.replace(/_\d+\(Clone\)$/, '').replace(/_\d+$/, '');
      const koreanDeptName = departmentAbbreviations2[departmentKey];  // 추출한 이름으로 한글 단과대 이름 찾기

      if (koreanDeptName && departmentCoordinates[koreanDeptName]) {
          const deptCoordinates = departmentCoordinates[koreanDeptName];
          const { newLatitude, newLongitude } = getRandomCoordinateWithinRadius(deptCoordinates.latitude, deptCoordinates.longitude);
  
          const monster = new Monster({
              name: name,
              dept: koreanDeptName,  // 한글 단과대 이름 사용
              generateSpot: koreanDeptName,  // 생성 위치에 단과대 이름 사용
              description: `${departmentAbbreviations[koreanDeptName]}에 주로 출몰함`,  // 설명에 한글 단과대 이름 사용
              tags: ['태그1', '태그2'],
              location: {
                  latitude: newLatitude,
                  longitude: newLongitude  // 단과대에 맞는 좌표
              },
              captured: false
          });
          await monster.save();
          monsters.push(monster);
      } else {
          console.log(`유효하지 않은 단과대 이름: ${departmentKey}`);
      }
  }
    

    return monsters;
}




  // 기존 랜덤 몬스터 생성 함수
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
    generateRandomMonsters,
    newgenerateRandomMonsters
  };
  