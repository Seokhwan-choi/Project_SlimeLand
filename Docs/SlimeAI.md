# 🧠 슬라임 AI 및 상태 머신 시스템

본 문서는 Slime Land 프로젝트 내에서 구현한 **슬라임의 AI 시스템 및 상태 머신 구조**에 대해 설명합니다.  
NavMesh 기반의 자연스러운 이동과 FSM(유한 상태 기계)을 통한 상태 전환을 통해,  
슬라임이 살아있는 듯한 반응과 움직임을 보일 수 있도록 설계되었습니다.

---

## 🔄 상태 머신(FSM) 기반 구조

### ✅ 주요 상태

| 상태 | 설명 |
|------|------|
| Idle | 기본 대기 상태 (앉기, 멍때리기 등 랜덤 애니메이션 포함) |
| Walk | NavMesh를 이용한 랜덤 위치로의 자연스러운 이동 |
| Interact | 플레이어의 터치나 선물에 반응하는 상태 |
| Sleep | 일정 시간 이상 무반응 시 슬라임이 자는 상태 |
| React | 놀람, 기쁨 등의 반응 상태 |

- 상태 전환은 타이머, 플레이어 입력, 확률 조건 등에 따라 유동적으로 전환됨
- 상태마다 진입/유지/종료 시 행동을 분리하여 관리함

---

## 🧭 NavMesh를 통한 자연스러운 이동

- Unity의 NavMeshAgent를 활용하여 **지형에 맞는 경로 탐색** 수행
- 걷기 속도, 회전 속도, 멈추는 위치 등을 슬라임 성격에 따라 세분화 가능
- NavMesh 경계 내 랜덤 위치를 타겟으로 하여 걷기 상태(Walk) 구현

```csharp
Vector3 target = GetRandomPointInNavMesh();
agent.SetDestination(target);
```

---

## 🛠️ 설계 의도

- **슬라임을 관찰하는 재미** 제공
- 단순 반복이 아닌 **상태 기반 행동 변화**를 통해 동적인 생명체로 연출
- 향후 다양한 감정 상태 및 행동 패턴 확장에 용이하도록 FSM 기반으로 설계

---

## 📁 관련 클래스

- `SlimeFSMController.cs`  
- `SlimeState_Idle.cs`  
- `SlimeState_Walk.cs`  
- `SlimeState_Interact.cs`  
- `SlimeState_Sleep.cs`  
- `SlimeMovement.cs`

