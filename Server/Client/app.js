function initWebSocket() {
    const socket = new WebSocket('ws://localhost:8000');
    let isMarkersVisible = false; // 몬스터 마커가 표시 중인지 추적
    let isCapturePending = false; // 캡처 중 여부를 확인하는 플래그

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
                    logMessage(`Requested to capture monster with ID: ${randomMonster._id}`);
                } else {
                    logMessage('No monsters available to capture.');
                }
                isCapturePending = false; // 캡처 완료 후 플래그 초기화
            }
        } else if (data.type === 'delete_all_success') {
            logMessage('All monsters have been deleted from the database.');
            removeAllMonsterMarkers(); // 모든 마커 제거
        }

        logMessage('Received from server: ' + event.data);
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

    // 랜덤 몬스터 생성 요청
    window.generateMonsters = function() {
        const message = {
            type: 'generate_monsters',
            count: 1 // 생성할 몬스터 수
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

    // 랜덤 몬스터 포획 요청
    window.captureAnyMonster = function() {
        logMessage('captureAnyMonster button clicked');
        isCapturePending = true; // 캡처 요청이 대기 중임을 표시
        getAllMonsters(); // 모든 몬스터 요청
    };

    // 마커 모두 제거 (getAllMonsters로 받은 데이터를 활용)
    function showAllMonsterMarkers(monsters) {
        monsters.forEach(monster => {
            addMonsterMarker(monster._id,monster.name, monster.location.latitude, monster.location.longitude);
        });
        logMessage('All monster markers added.');
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
