const User = require('./models/User');
const Monster = require('./models/Monster');
const { getDistanceFromLatLonInMeters, generateRandomMonsters } = require('./utils');

async function handleMessage(ws, message) {
  try {
    const data = JSON.parse(message);

    // 랜덤 몬스터 더미 데이터 생성
    if (data.type === 'generate_monsters') {
      const monsters = await generateRandomMonsters(data.count || 5);
      ws.send(JSON.stringify({
        type: 'generate_monsters_success',
        message: `${monsters.length} monsters created successfully.`,
        monsters: monsters
      }));
      console.log(`${monsters.length} monsters generated and stored in DB.`);
      console.log(monsters);
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
}

module.exports = {
  handleMessage
};
