const mongoose = require('mongoose');
const Spot = require('./models/Spot');
require('dotenv').config();

// MongoDB 연결 URI 설정
const mongoURI = process.env.MONGODB_URI;

if (!mongoURI) {
    console.error('MONGODB_URI 환경 변수가 설정되지 않았습니다.');
    process.exit(1);
}

// 삽입할 데이터 정의
// const spotsData = {
//   '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
//   '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
//   '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
//   '체육대학': { latitude: 37.244493, longitude: 127.080436 },
//   '공과대학': { latitude: 37.246468, longitude: 127.080844 },
//   '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
//   '외국어대학': { latitude: 37.245391, longitude: 127.077649 },
//   '생명과학대학': { latitude: 37.242962, longitude: 127.080932 }
// };
const spotsData = {
  '중앙도서관': {latitude:37.241047, longitude:127.079659 }
};

// 비동기 함수로 데이터 삽입
async function insertSpots() {
  try {
    // MongoDB 연결
    await mongoose.connect(mongoURI, {
      useNewUrlParser: true,
      useUnifiedTopology: true
    });
    console.log('MongoDB에 연결되었습니다.');

    // 삽입할 문서 배열 생성
    const spots = Object.entries(spotsData).map(([dept, coords]) => ({
      spotName: dept,
      dept: dept,
      description: '',     // 필요에 따라 기본값 설정
    //   lastPoint: '',       // 기본값 0으로 적용
      location: coords,
      capturedBy: ''       // 필요에 따라 기본값 설정
    }));

    // 데이터 삽입
    await Spot.insertMany(spots);
    console.log('스팟 데이터가 성공적으로 삽입되었습니다.');
  } catch (error) {
    console.error('스팟 데이터 삽입 중 오류 발생:', error);
  } finally {
    // MongoDB 연결 종료
    await mongoose.disconnect();
    console.log('MongoDB 연결이 종료되었습니다.');
  }
}

// 함수 실행
insertSpots();
