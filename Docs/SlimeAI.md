# 🧠 슬라임 AI 및 상태 머신 시스템

이 문서는 Slime Land 프로젝트에 등장하는 **슬라임의 AI 시스템**과 **상태 머신 기반 동작 구조**에 대해 설명합니다.  
NavMesh 기반 이동과 감정 표현, 상호작용을 통해 **생명력 있는 슬라임 캐릭터**를 구현했습니다.

---

## ⚙️ 핵심 구조

- 슬라임은 기본적으로 다음 3가지 상태를 가집니다:

| 상태 | 설명 |
|------|------|
| `Idle` | 제자리에서 감정을 표현하거나 멈춰있는 상태 |
| `RandomMove` | 주변을 자유롭게 돌아다니는 상태 |
| `Communication` | 플레이어의 상호작용에 반응하는 상태 |

---

## 😊 Idle 상태의 감정 표현

- `Idle` 상태는 단순 정지 상태가 아닌, **풍부한 감정 애니메이션**이 표현되는 상태입니다
- `EmotionType`을 기반으로 슬라임은 다양한 감정을 표현합니다:

| EmotionType | 설명 |
|-------------|------|
| `Idle`    | 일반적인 대기 상태 |
| `Excited` | 기분 좋을 때, 활발하게 |
| `Happy`   | 긍정적인 감정 표현 |
| `Shock`   | 깜짝 놀란 반응 |
| `Sleepy`  | 졸린 상태 표현 |

- 각 감정은 랜덤 또는 이벤트에 따라 전환되며, 슬라임의 **개성**을 강조합니다

---

## 🐾 이동 동작 로직 (NavMesh)

- `RandomMove` 상태에서는 **NavMeshAgent**를 통해 슬라임이 주변을 자연스럽게 이동
- 목적지 도달 후 일정 시간 대기 → 다음 위치 탐색을 반복
- 장애물 회피, 속도 설정 등은 Agent 속성으로 조정

```csharp
if (mActive)
{
    if (mPathFindTarget != null)
    {
        if (mPathFindTargetPos.Value != mPathFindTarget.position)
        {
            mPathFindTargetPos = mPathFindTarget.position;

            mAgent.SetDestination(mPathFindTargetPos.Value);
        }
    }

    if (mAgent.isStopped == false)
    {
        mFlipInterval -= dt;
        if (mFlipInterval <= 0f)
        {
            Vector3 dir = mAgent.velocity.normalized;

            bool isRight = dir.x > 0;

            mCharacter.ChangeSpriteFlipX(isRight);

            mFlipInterval = 0.25f;
        }
    }

    if (mArriveAction != null && IsArrive())
    {
        mArriveAction.Invoke();

        ResetPath();
    }
}
```

![Image](https://github.com/user-attachments/assets/a110835f-3365-4e79-b8df-2971dce3d884)

---

## 💡 설계 의도

- 단순 반복 이동이 아닌 **감정 기반 슬라임 캐릭터성** 부여
- AI 상태 간 전환을 통해 힐링 게임의 생동감을 구현

---

## 📁 관련 클래스
- `CharacterAI.cs`  
- `CharacterPathFinder.cs`  
- `CharacterAnim.cs`  
- `ActionManager.cs`
- `CharacterAction.cs`
- `Action_RandomMove.cs`
- `Action_Communication.cs`
- `Action_Idle.cs`
