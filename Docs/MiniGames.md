# 🎮 슬라임 미니게임 시스템

본 문서는 Slime Land 프로젝트에 포함된 **2종의 미니게임**에 대해 설명합니다.  
단순한 방치형 플레이 외에도 **가볍고 재미있는 상호작용 콘텐츠**를 제공하기 위해 기획되었습니다.

---

## 1️⃣ 틱택토 (Tic Tac Toe)

- **3x3 보드**에서 플레이어와 슬라임(AI)이 번갈아가며 O/X를 배치
- 게임 시작 시 플레이어 또는 슬라임 중 무작위로 선공 결정
- 승리 조건: **가로 / 세로 / 대각선** 중 하나로 같은 기호 3개 연속 배치
- 게임 종료 시 결과(승리/패배/무승부) 애니메이션 출력

### 🤖 AI 전략 - Minimax 알고리즘

- 슬라임 AI는 단순 랜덤이 아닌 **Minimax 알고리즘** 기반 전략 사용
- 가능한 모든 수를 탐색하여 최적의 수를 선택
- 플레이어가 어떤 수를 두더라도 **최소한 무승부** 또는 **최선의 승리**를 도출하도록 설계

```text
Minimax는 가능한 모든 수를 시뮬레이션하여
상대가 최선의 수를 둔다고 가정한 상황에서도 
자신에게 가장 유리한 선택을 하는 알고리즘입니다.
```

> Minimax는 컴퓨터가 틱택토처럼 상태 공간이 적은 게임에서 완벽한 플레이를 수행할 수 있게 해줍니다.

---

## 2️⃣ 속성 수업: 니편내편 (팀 분류 게임)

- 다양한 속성을 가진 슬라임들이 등장
- 플레이어는 슬라임을 **속성별로 좌우로 나누어 정렬**해야 함
- 제한 시간 내 **더 많은 정답을 맞출수록 점수 상승**
- 일정 점수 이상을 도달하면 보상획득

### 🧠 구현 요소

- 속성별 정답 검증 로직
- 난이도/속도 증가 기능 포함

<p align="center">
  <img src="https://github.com/user-attachments/assets/bc064836-1239-41be-83d3-00090cd046ff" width="280" style="margin-right: 16px;" />
<img src="https://github.com/user-attachments/assets/0adbe006-54be-442a-95b2-ac6094e0886d" width="280" style="margin-right: 16px;" />
  <img src="https://github.com/user-attachments/assets/09e5c4cb-5c04-46bf-8cc0-ed9b50a4ad5c" width="280"/>
</p>

---

## 💡 설계 의도

- 힐링 중심의 게임 흐름에 **짧은 몰입 콘텐츠** 제공
- 슬라임 캐릭터에 대한 **관찰력과 반응성**을 자극
- 반복성에 변화를 주며 **유저 리텐션** 유도

---

## 📁 관련 클래스

- `Popup_MiniGameHomeUI.cs`  
- `Popup_MiniGame_TicTacToeUI.cs`  
- `TicTacToe_MiniMaxAI.cs` (Minimax 알고리즘 포함)
- `Popup_MiniGame_ElementalCoursesUI.cs`  
- `ElementalCourses_ScoreManager.cs`  
- `ElementalCourses_FeverManager.cs`
- `ElementalCourses_TimeMananger.cs`
- `ElementalCourses_TeamManager.cs`
