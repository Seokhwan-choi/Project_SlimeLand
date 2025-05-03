# 👗 슬라임 코스튬 시스템

본 문서는 Slime Land 프로젝트 내에서 구현한 **슬라임의 코스튬 시스템**에 대해 설명합니다.  
머리, 몸통, 악세서리 등 다양한 부위를 꾸미는 기능을 통해 **커스터마이징의 즐거움**을 제공합니다.

---

## 🧩 코스튬 구조

- 코스튬은 **슬라임의 부위별로 장착**됩니다:
  - 얼굴 (Face)
  - 몸통 (Body)
  - 악세서리 (Accessory)

- 코스튬은 각기 다른 위치에 장착되어야 하므로,
  **슬라임마다 부위별 좌표를 직접 지정**하여 배치

---

## 🔧 좌표 기반 장착 방식

- `CostumePosData.cs`를 통해 각 코스튬의 위치와 방향을 설정
- 슬라임의 종류별로 좌표와 회전값을 수작업으로 설정
- 다양한 형태의 슬라임에 대응 가능하지만, **유지보수의 어려움** 존재

```csharp
string costume = slimeInfo.Costumes[(int)mType];
if (costume.IsValid())
{
    gameObject.SetActive(true);
    CostumeData costumeData = MLand.GameData.CostumeData.TryGet(costume);
    if (costumeData != null)
    {
        // 코스튬 장착
        var renderer_idle_00 = gameObject.FindComponent<SpriteRenderer>($"{mType}_Idle_00");
        var renderer_idle_01 = gameObject.FindComponent<SpriteRenderer>($"{mType}_Idle_01");

        renderer_idle_00.sprite = MLand.Atlas.GetCostumeSprite(costumeData.spriteImg);
        renderer_idle_01.sprite = MLand.Atlas.GetCostumeSprite(costumeData.spriteImg2);

        var costumePosData = DataUtil.GetCostumePosData(mSlimeId, costume);
        if (costumePosData != null)
        {
            string posStr00 = string.Empty;
            string posStr01 = string.Empty;
            // Move, Idle, Excited, Happy, Shock, Sleepy 중 하나 사용
            if (animType == AnimType.Excited)
            {
                posStr00 = costumePosData.pos[(int)CostumePos.Excited];
            }
            else if (animType == AnimType.Happy)
            {
                posStr00 = costumePosData.pos[(int)CostumePos.Happy];
            }
            else if (animType == AnimType.Shock)
            {
                posStr00 = costumePosData.pos[(int)CostumePos.Shock];
            }
            else if (animType == AnimType.Sleepy)
            {
                posStr00 = costumePosData.pos[(int)CostumePos.Sleepy];
            }
            else // Move, Idle
            {
                // idle 00 과 idle 01 사용
                posStr00 = costumePosData.pos[(int)CostumePos.Idle00];
                posStr01 = costumePosData.pos[(int)CostumePos.Idle01];
            }

            Pos pos00 = Pos.Parse(posStr00);
            Pos pos01 = Pos.Parse(posStr01);

            // 코스튬 위치 조정
            renderer_idle_00.transform.localPosition = new Vector3(pos00.X, pos00.Y);
            renderer_idle_01.transform.localPosition = new Vector3(pos01.X, pos01.Y);

            // 코스튬 댑스 조정
            renderer_idle_00.sortingOrder = costumePosData.orderInLayer;
            renderer_idle_01.sortingOrder = costumePosData.orderInLayer;
        }
    }
}
else
{
    gameObject.SetActive(false);
}
```

<p align="center">
  <img src="https://github.com/user-attachments/assets/60ed4b74-f8f8-45da-8ca6-e3cab539d849" width="280" style="margin-right: 16px;" />
  <img src="https://github.com/user-attachments/assets/3074f5a7-7428-4ebe-b0aa-7f17c750e34d" width="280"/>
</p>

---

## 🧠 기술 개선 방향

- 기본적을 첫 설계부터 코스튬을 추가하는 것을 고려하지 않는 구조로 작업을 진행한게 문제점으로 작용
- Sprite Resolver라는 기능을 알게되었지만 현재 구조상 적용에 어려움이 있었음
- 추후 프로젝트에서는 Sprite Resolver 기반의 구조로 개선하여, **유지보수성 및 유연성 확보**
- [랜스 키우기 코스튬 시스템](ProJect_Lance/Docs/CostumeSystem.md)

---

## 💡 설계 의도

- 단순 외형 변경이 아닌, 유저와 슬라임 간의 **개성 표현 수단** 제공
- 힐링 게임의 정체성을 강화하고, **꾸미기 요소**를 통해 반복 플레이 유도
- 다양한 코스튬 테마(계절, 직업, 감정 등)로 확장 가능

---

## 📁 관련 클래스

- `CharacterAnim.cs`  
- `CostumeData.cs`
- `CostumePosData.cs`
