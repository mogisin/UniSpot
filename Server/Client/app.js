function initWebSocket() {
    const socket = new WebSocket('ws://localhost:8000');
    let isMarkersVisible = false; // 몬스터 마커가 표시 중인지 추적
    let isCapturePending = false; // 캡처 중 여부를 확인하는 플래그
    let nearbyMonsters = [];
    // 로그 출력 함수
    function logMessage(message) {
        const logDiv = document.getElementById('log');
        logDiv.innerHTML += `<p>${message}</p>`;
        logDiv.scrollTop = logDiv.scrollHeight;
    }

    // WebSocket 연결이 열리면 실행
    socket.onopen = function() {
        logMessage('WebSocket connection opened.');
    };

    // 서버로부터 메시지 수신
    socket.onmessage = function(event) {
        const data = JSON.parse(event.data);
        if (data.type === 'get_all_monsters') {
            if (isMarkersVisible) {
                showAllMonsterMarkers(data.monsters); // 몬스터 마커를 보여줌
            }

            // 랜덤 몬스터 포획 대기 중이면 처리
            if (isCapturePending) {
                const monsters = data.monsters;
                if (monsters.length > 0) {
                    const randomMonster = monsters[Math.floor(Math.random() * monsters.length)];
                    const captureMessage = {
                        type: 'capture',
                        username: 'testUser',
                        monsterId: randomMonster._id
                    };
                    socket.send(JSON.stringify(captureMessage));
                    logMessage(`get_all_monsters: Requested to capture monster with ID: ${randomMonster._id}`);
                } else {
                    logMessage('get_all_monsters: No monsters available to capture.');
                }
                isCapturePending = false; // 캡처 완료 후 플래그 초기화
            }
        } else if (data.type === 'delete_all_success') {
            logMessage('All monsters have been deleted from the database.');
            removeAllMonsterMarkers(); // 모든 마커 제거

        } else if (data.type === 'nearby_monsters'){
            nearbyMonsters = data.monsters;
            if (isCapturePending){
                if (nearbyMonsters.length>0){
                    logMessage(`가까운 몬스터 id : ${nearbyMonsters[0]._id}`)
                    
                    captureMonster(nearbyMonsters[0]); // 몬스터 객체 전송
                } else{
                    logMessage('No nearby monsters to capture.');
                isCapturePending = false; // 대기 상태 해제
                }
            }
             else { 
                logMessage('No Pending Request about Capture');
            } 

        } else if(data.type==='res_nearby_monsters'){
            nearbyMonsters = data.monsters;
            logMessage('Received res_nearby_monsters');
            // logMessage(nearbyMonsters[0].name);

        } else if(data.type === 'res_user_money'){
            const {money} = data;
            logMessage(`res_user_money: 현재 유저 money : ${money}`);
        }
        else{
            logMessage('Received from server: ' + event.data);
        }

    };

    // WebSocket 연결이 닫힐 때
    socket.onclose = function() {
        logMessage('WebSocket connection closed.');
    };

    // WebSocket 에러 발생 시
    socket.onerror = function(error) {
        logMessage('WebSocket error: ' + error.message);
    };

    // 위치 정보 전송 함수
    window.sendLocation = function(department) {
        const departmentCoordinates = {
            '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
            '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
            '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
            '체육대학': { latitude: 37.244493, longitude: 127.080436 },
            '공과대학': { latitude: 37.246468, longitude: 127.080844 },
            '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
            '외국어대학': { latitude: 37.245391, longitude: 127.077649 },
            '생명과학대학': { latitude: 37.242962, longitude: 127.080932 }
        };

        const location = departmentCoordinates[department];
        if (location) {
            const message = {
                type: 'location',
                username: 'testUser',
                latitude: location.latitude,
                longitude: location.longitude
            };
            socket.send(JSON.stringify(message));
            logMessage('Sent location for ' + department + ': ' + JSON.stringify(message));
        }
    };
    window.updateLocation = function(department){
        const departmentCoordinates = {
            '소프트웨어융합대학': { latitude: 37.239603, longitude: 127.083157 },
            '전자정보대학': { latitude: 37.239782, longitude: 127.083313 },
            '응용과학대학': { latitude: 37.239811, longitude: 127.083476 },
            '체육대학': { latitude: 37.244493, longitude: 127.080436 },
            '공과대학': { latitude: 37.246468, longitude: 127.080844 },
            '예술디자인대학': { latitude: 37.241709, longitude: 127.084441 },
            '외국어대학': { latitude: 37.245391, longitude: 127.077649 },
            '생명과학대학': { latitude: 37.242962, longitude: 127.080932 }
        };
        const location = departmentCoordinates[department];
        if(location){
            const message = {
                type:'updateLocation',
                username:'testUser',
                latitude:location.latitude,
                longitude:location.longitude
            }
            socket.send(JSON.stringify(message));
            logMessage("Sent updateLocation type message");
        }
    };

    // 랜덤 몬스터 생성 요청
    window.generateMonsters = function() {
        const message = {
            type: 'generate_monsters',
            count: 5 // 생성할 몬스터 수
        };
        socket.send(JSON.stringify(message));
        logMessage('Requested to generate random monsters: ' + JSON.stringify(message));
    };

    // 모든 몬스터의 위치 요청
    window.getAllMonsters = function() {
        const message = {
            type: 'get_all_monsters'
        };
        socket.send(JSON.stringify(message));
        logMessage('Requested to get all monsters.');
    };

    // 모든 몬스터 삭제 요청
    window.deleteAllMonsters = function() {
        const message = {
            type: 'delete_all'
        };
        socket.send(JSON.stringify(message));
        logMessage('Requested to delete all monsters.');
    };
    window.getUserMoney = function(username){
        const message = {
            type: 'get_user_money',
            username:username
        }
        socket.send(JSON.stringify(message));
        logMessage('get_user_money : 유저 money 데이터 요청 보냄');
    }
    window.captureMonsterNEW = function(username){
        const message = {
            type: 'capture_monster',
            username:username,
            monsterName: 'Software_1(Clone)'
        };
        socket.send(JSON.stringify(message));
        logMessage('capture_monster : 새 형식 데이터 요청 보냄');
    }

    // 랜덤 몬스터 포획 요청
    window.captureAnyMonster = function() {
        logMessage('captureAnyMonster button clicked');
        isCapturePending = true; // 캡처 요청이 대기 중임을 표시
        getAllMonsters(); // 모든 몬스터 요청
    };
    
    //가까운 랜덤 몬스터 포획 요청
    window.captureNearbyMonster = function(username){
        logMessage('captureNearbyMonster button clicked');
        isCapturePending = true
        // sendLocation('소프트웨어융합대학'); // 일단 소융대 주변 가까운 몬스터 포획 시도

        //유저위치 업데이트 이후
        getNearbyMonsters(username);
        captureMonster(nearbyMonsters[0]);

        
        // const captureMessage = {
        //     type:'capture',
        //     username: 'testUser',
        //     monsterId: nearbyMonsters[0]._id
        // }
        // socket.send(JSON.stringify(captureMessage));
        // logMessage(`Capture : Requested to capture monster with ID: ${nearbyMonsters[0]._id}`);
    }
    window.getNearbyMonsters = function(username){
        logMessage('Get Nearby Monster button clicked');
        const message = {
            type:'get_nearby_monsters',
            username: username
        }
        socket.send(JSON.stringify(message));
        logMessage('sent get_nearby_monsters type message');

    }

    // 마커 모두 제거 (getAllMonsters로 받은 데이터를 활용)
    function showAllMonsterMarkers(monsters) {
        monsters.forEach(monster => {
            addMonsterMarker(monster._id,monster.name, monster.location.latitude, monster.location.longitude);
        });
        logMessage('All monster markers added.');
    }
    function captureMonster(monsterToCapture){
        // const monsterToCapture = nearbyMonsters[0];
        const captureMessage = {
            type:'capture',
            username: 'testUser',
            monsterId: monsterToCapture._id
        }
        socket.send(JSON.stringify(captureMessage));
        logMessage(`Capture : Requested to capture monster: ${monsterToCapture.name}, ${monsterToCapture._id}`);
    
        isCapturePending = false; // 대기 상태 해제
    }

    // 토글 버튼 클릭 시 실행되는 함수
    window.toggleMonsterMarkers = function() {
        if (isMarkersVisible) {
            // 마커 숨기기
            removeAllMonsterMarkers();
        } else {
            // 마커 보여주기
            getAllMonsters();
        }
        isMarkersVisible = !isMarkersVisible; // 상태 변경
    };
}
