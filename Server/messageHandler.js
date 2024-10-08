const User = require('./models/User');
const Monster = require('./models/Monster');
const Spot = require('./models/Spot')
const { getDistanceFromLatLonInMeters, generateRandomMonsters, newgenerateRandomMonsters } = require('./utils');


async function handleMessage(ws, message) {
  try {
    const data = JSON.parse(message);

    // 랜덤 몬스터 더미 데이터 생성
    if (data.type === 'generate_monsters') {
      const monsters = await newgenerateRandomMonsters(data.count || 1);
      // const monsters = await generateRandomMonsters(data.count || 1);

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
        user = new User({ 
          username,
          money: 0, 
          location: { latitude, longitude } });
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

    } else if(data.type === 'updateLocation'){
      const { username, latitude, longitude } = data;
      let user = await User.findOne({ username });
      
      user.location.latitude = latitude;
      user.location.longitude = longitude;
      await user.save();
      console.log('updateLocation : 유저 위치 업데이트')

    } else if (data.type === 'capture') { //몬스터 잡았을 때
      const { username, monsterId } = data;

      // 몬스터 포획 처리
      const monster = await Monster.findById(monsterId);
      if (monster && !monster.captured) {
        monster.captured = true;
        await monster.save();
        console.log('몬스터 포획 완료:', monster);
        // 유저의 몬스터 목록에 추가


        let user = await User.findOne({ username });
        user.money += 100;
        await user.save();

        user.monsters.push({ 
          name: monster.name, 
          capturedAt: new Date(),
          monsterId:monster._id
        });
        await user.save();

        console.log('유저 몬스터 목록 업데이트:', user);

        // 몬스터 포획 성공 메시지 전송
        ws.send(JSON.stringify({
          type: 'capture_success',
          monster: monster.name
        }));
      }
    } else if (data.type ==='get_all_monsters'){ //모든 몬스터 정보 받아오기
      const allMonsters = await Monster.find({ captured: false });

      const response ={
        type:'get_all_monsters',
        monsters:allMonsters
      };
      // console.log('모든 몬스터 출력 시작')
      // console.log(allMonsters)
      // console.log('모든 몬스터 출력 종료')
      ws.send(JSON.stringify(response))
    }

    else if (data.type === 'delete_all') { //몬스터 모두 삭제
      // 모든 몬스터 삭제
      await Monster.deleteMany({});
      console.log('All monsters deleted from the database.');
  
      // 모든 유저의 몬스터 목록 비우기
      await User.updateMany({}, { $set: { monsters: [] } });
      console.log('All users\' monster lists cleared.');
  
      // 클라이언트에 삭제 완료 메시지 전송
      ws.send(JSON.stringify({
          type: 'delete_all_success',
          message: 'All monsters and user monster lists have been deleted.'
      }));
    }

    else if (data.type === 'delete_monster') { //몬스터 하나 삭제
          const { monsterId } = data;
      
          // 몬스터 삭제
          const deletedMonster = await Monster.findByIdAndDelete(monsterId);
          if (deletedMonster) {
              // 모든 유저의 몬스터 목록에서 해당 몬스터 삭제
              await User.updateMany(
                  { "monsters.name": deletedMonster.name }, // 몬스터 이름으로 찾음
                  { $pull: { monsters: { name: deletedMonster.name } } } // 몬스터 삭제
              );
              console.log(`Monster with ID ${monsterId} deleted from database and user data.`);
      
              // 클라이언트에 몬스터 삭제 메시지 전송
              ws.send(JSON.stringify({
                  type: 'delete_monster',
                  monsterId: monsterId
              }));
          } else {
              ws.send(JSON.stringify({
                  type: 'error',
                  message: `Monster with ID ${monsterId} not found.`
              }));
          }
      } 
      else if (data.type === 'capture_spot'){
        const {username, spotName} = data;
        let spot = await Spot.findOne({ spotName });
        let user = await User.findOne({ username });

        if (!spot || !user) {
          console.error("Spot or User not found.");
          return;
        }

        let userPoint = user.monsters.length;

        if (spot.capturedBy != username){

          if (userPoint > spot.lastPoint){
            spot.capturedBy = username;
            await spot.save();
            user.monsters = [];
            await user.save();

            console.log(`${username} captured ${spotName}`);
            ws.send(JSON.stringify({
              type:"capture_spot_success",
              spotName:spotName,
              username:username
            }))
          } else {
            console.log(`${username} does not have enough points to capture ${spotName}`);
          }

        } else {
          console.log(`${username} already captured ${spotName}`);
        }

      } else if(data.type === 'get_nearby_monsters'){
        const { username} = data;
        const user = await User.findOne({username});
        const allMonsters = await Monster.find({ captured: false });

        // 반경 10미터 이내의 몬스터 필터링
        const nearbyMonsters = allMonsters.filter(monster => {
          const distance = getDistanceFromLatLonInMeters(
            user.location.latitude,
            user.location.longitude,
            monster.location.latitude,
            monster.location.longitude
          );
          return distance <= 10; // 10미터 이내인지 확인
        });

        console.log('반경 10미터 이내의 주변 몬스터:', nearbyMonsters);
        ws.send(JSON.stringify({
          type: 'res_nearby_monsters',
          monsters: nearbyMonsters
        }));

      } else if(data.type === 'nearby_monsters'){
        const { username, latitude, longitude } = data;
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
        ws.send(JSON.stringify({
          type: 'nearby_monsters',
          monsters: nearbyMonsters
        }));
        
      } else if(data.type === 'get_user_money'){
        // console.log(data);
        const{username} = data;
        let user = await User.findOne({username});
        // console.log(username);
        // 유저의 위치 정보 업데이트
        if (!user) {
          user = new User({ 
            username,
            money: 0, 
            location: { latitude:11, longitude:11 } });
          console.log('새로운 유저 생성:');
        } 
        await user.save();
        // console.log('유저 저장 완료:');


        const message = {
          type:'res_user_money',
          username: user.username,
          money: user.money
        }
        ws.send(JSON.stringify(message));
        console.log(`res_user_money : money : ${user.money}`);

      } else if(data.type === 'update_user_money'){
        console.log(data)
        const {username, money} = data;
        let user = await User.findOne({username});

        user.money = money;
        await user.save();
        console.log(`현재 유저 money : ${user.money}`);

      } else if (data.type === 'capture_monster') {
        console.log(data);
        const { username, monsterName } = data;
    
        // 유저 정보 조회
        let user = await User.findOne({ username });
        
        // 새로운 몬스터 생성 (아마 배열로 반환될 가능성 있음, 확인 필요)
        const newMonsters = await newgenerateRandomMonsters(1, monsterName);
        const newMonster = newMonsters[0]; // 첫 번째 몬스터 선택
        // console.log(newMonster);
        
        if (newMonster) {
            console.log(`생성된/포획된 몬스터 : ${newMonster.name}, ${newMonster._id}`);
    
            // 유저의 몬스터 목록에 추가
            user.monsters.push({ 
                name: newMonster.name, 
                capturedAt: new Date(),
                monsterId: newMonster._id
            });
            await user.save();
        } else {
            console.error('새로 생성된 몬스터를 찾을 수 없습니다.');
        }
    } else if (data.type === 'get_duplicate_name'){
      console.log(data.username);
  try {
    // 해당 유저의 몬스터 목록을 가져옴
    const user = await User.findOne({ username: data.username });

    if (!user) {
      console.log('유저를 찾을 수 없습니다.');
      return;
    }

    // 유저가 소유한 몬스터 목록에서 중복된 이름 확인
    const monsterNames = await User.aggregate([
      {
        $match: { username: data.username }  // 해당 유저의 데이터만 선택
      },
      {
        // monsters 배열을 펼쳐서 각 몬스터 객체를 도큐먼트로 취급
        $unwind: "$monsters"
      },
      {
        // name 필드로 그룹화
        $group: {
          _id: "$monsters.name",  // 그룹화 기준: monsters 배열의 name 필드
          count: { $sum: 1 }  // 해당 이름을 가진 몬스터 수 계산
        }
      },
      {
        // 모든 결과를 출력 (중복이 없더라도 포함)
        $project: {
          _id: 0,  // _id 필드 숨기기
          name: "$_id",  // name 필드에 _id 값 할당
          count: 1  // count 필드 유지
        }
      }
    ]);

    const message = {
      type: 'res_user_monster_names',
      monsters: monsterNames
    };

    ws.send(JSON.stringify(message));

    console.log('유저가 보유한 몬스터 이름 및 개수:', monsterNames);
  } catch (err) {
    console.error('중복 확인 오류 발생:', err);
  }
    }
    


  } catch (error) {
    console.error('Error processing message:', error);
  }
}

module.exports = {
  handleMessage
};
