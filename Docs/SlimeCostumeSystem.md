# 👗 슬라임 코스튬 시스템

이 문서는 Slime Land 프로젝트 내에서 구현한 **슬라임의 코스튬 시스템**에 대해 설명합니다.  
머리, 몸통, 악세서리 등 다양한 부위를 꾸미는 기능을 통해 **커스터마이징의 즐거움**을 제공합니다.

---

## 🧩 코스튬 구조

- 코스튬은 **슬라임의 부위별로 장착**됩니다:
  - 머리 (Head)
  - 몸통 (Body)
  - 악세서리 (Accessory)

- 코스튬은 각기 다른 위치에 장착되어야 하므로,
  초기에는 **슬라임마다 부위별 좌표를 직접 지정**하여 배치

---

## 🔧 좌표 기반 장착 방식

- `SlimeCostumeManager.cs`를 통해 각 코스튬의 위치와 방향을 설정
- 슬라임의 종류별로 좌표와 회전값을 수작업으로 설정
- 다양한 형태의 슬라임에 대응 가능하지만, **유지보수의 어려움** 존재

```csharp
costumeTransform.localPosition = slimePreset.headPosition;
costumeTransform.localRotation = slimePreset.headRotation;
```

---

## 🧠 기술 개선 방향

- 개발 후반 Sprite Resolver의 존재를 확인함
- `SpriteResolver`를 활용하면 **슬라임 프리셋에 따라 자동 위치 조정** 가능
- 추후 프로젝트에서는 Sprite Resolver 기반의 구조로 개선하여, **유지보수성 및 유연성 확보**

---

## 💡 설계 의도

- 단순 외형 변경이 아닌, 유저와 슬라임 간의 **개성 표현 수단** 제공
- 힐링 게임의 정체성을 강화하고, **꾸미기 요소**를 통해 반복 플레이 유도
- 다양한 코스튬 테마(계절, 직업, 감정 등)로 확장 가능

---

## 📁 관련 클래스

- `SlimeCostumeManager.cs`  
- `SlimePreset.cs`  
- `CostumeData.cs`  
- (향후) SpriteResolver 기반 시스템
