![ChatGPT Image 2025년 5월 21일 오전 07_58_25](https://github.com/user-attachments/assets/78780fee-a54d-4b52-90dc-0bca75f68ba7)

## 📘 목차
- **[프로젝트 소개](#-프로젝트-소개)**
- **[개발 기간](#-개발-기간)**
- **[사용 기술](#-사용-기술)**
- **[핵심 로직](#-핵심-로직)**

<br/>

---

## 🔍 프로젝트 소개
**ImpossibleBosses**는 **Unity 6** 기반으로 개발된 **멀티플레이 PvE 보스 레이드 게임**입니다.  
플레이어는 최대 **8명**까지 팀을 이루어 거대한 보스 몬스터에 도전하며,  
**아처(Archer)**, **파이터(Fighter)**, **메이지(Mage)**, **몽크(Monk)**, **네크로맨서(Necromancer)** 중  
총 **5개 클래스** 중 하나를 선택할 수 있습니다.

보스의 다양한 공격 패턴을 팀원 간 **협동과 전략**으로 공략하며,  
전투를 통해 **아이템을 획득**하고, **장비를 교체**하면서  
플레이어는 점차 강해져 **최종 보스를 처치하는 것**이 목표입니다.

<br/>

---

## 📆 개발 기간
<b>2024-12-11 ~</b>

<br/>

---

## 🔧 사용 기술
| 구분 | 기술명 |
| :-- | :-- |
| **Engine** | `Unity 6` |
| **Networking** | `Unity Netcode` |
| **Voice & Chat** | `Vivox Service` |
| **Auth / DB** | `Google OAuth 2`, `Google Spreadsheet` |
| **AI Behavior** | `Behaviour Tree Designer` |
| **Test Tool** | `Unity Play Mode Scenarios` |

<br/>

---

## 🗝 핵심 로직

### 🔐 로그인
1. **`LoginScene`** 로드  
2. 플레이어가 **로그인 / 회원가입** 선택  
3. `LogInManager`가 **Google 스프레드시트**(UserAuthenticateData)와 ID·PW 대조  
4. **인증 성공 → 로비 씬**으로 이동  

![Image](https://github.com/user-attachments/assets/a63eec10-7526-4920-bd92-319d0a640e82)

---

### 📥 데이터 불러오기
ImpossibleBosses의 데이터 관리는 Managers.DataManager를 중심으로 이루어집니다. 이 매니저는 Google 스프레드시트에서 게임에 필요한 각종 데이터(플레이어 정보, 몬스터 정보, 아이템 목록 등)를 로드하고, 애플리케이션 내에서 효율적으로 사용할 수 있도록 처리합니다.

<p align="center">
  <strong>&lt;데이터 시트&gt;</strong>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/5fa4ab70-ba04-403b-b229-c403439998e1" alt="데이터 시트 이미지 1" width="70%"/>
  <img src="https://github.com/user-attachments/assets/66638094-07c2-48ad-b48e-744c3f8d9183" alt="데이터 시트 이미지 2" width="70%"/>
  <img src="https://github.com/user-attachments/assets/8c104aa5-92eb-44ea-82e5-c77787290c39" alt="데이터 시트 이미지 3" width="70%"/>
</p>


**데이터 로딩 절차:**

1.  **초기화 및 타입 스캔**:
    <ul>
      <li><code>Managers.DataManager.Init()</code> 메서드가 데이터 로딩을 시작합니다.</li>
      <li><code>LoadSerializableTypesFromFolder</code> 메서드는 지정된 경로에서 <code>[Serializable]</code> 어트리뷰트를 가진 클래스들을 리플렉션으로 스캔합니다. 이 클래스들은 스프레드시트의 각 시트 데이터 구조와 매핑됩니다.</li>
    </ul>

<p align="center">
  <strong>&lt;DataManger의 타입확인&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/f0fcfdb3-cd07-494a-9edd-267df547bfd1" alt="타입 스캔 이미지 1" width="70%"/>
</div>
<br>

2.  **Google 스프레드시트 연동**:
    <ul>
      <li><code>DatabaseStruct</code>는 Google OAuth 2.0 인증 정보(클라이언트 ID, 시크릿 코드, 애플리케이션 이름, 스프레드시트 ID)를 관리합니다.</li>
      <li><code>GetGoogleSheetData()</code> 메서드는 이 정보를 사용하여 Google Sheets API 인증 후, 지정된 스프레드시트 데이터를 가져옵니다.</li>
    </ul>

<p align="center">
  <strong>&lt;구글 스프레드시트 불러오기&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/47be08dd-43b7-4740-9853-89e74ab992f3" alt="스프레드시트 연동" width="70%"/>
</div>
<br>

3.  **데이터 파싱 및 구조화**:
    * `LoadDataFromGoogleSheets()`는 인증된 서비스와 스프레드시트 ID로 각 시트의 데이터를 요청합니다.
    * `ParseSheetData()`는 시트 데이터를 JSON 형식 문자열로 변환합니다.
    * `AddAllDataDictFromJsonData()`는 JSON 문자열을 C# 객체로 역직렬화합니다.
        * `GetTypeNameFromFileName()`은 시트 이름에서 데이터 타입을 결정합니다.
        * `FindGenericKeyType()`은 데이터 타입이 `Ikey<TKey>` 인터페이스를 구현했는지 확인하여 딕셔너리 키 타입을 결정합니다.
        * `DataToDictionary<TKey, TStat>` 클래스는 로드된 데이터 리스트를 `Dictionary<TKey, TStat>` 형태로 변환하여 `AllDataDict`에 저장합니다.

<p align="center">
  <strong>&lt;JSON 문자열 역직렬화&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/5d878784-0047-4341-a7d5-eaf3cad0e707" alt="데이터 파싱" width="70%"/>
</div>
<br>

4.  **데이터 캐싱 및 접근**:
    * 처리된 데이터는 `DataManager.AllDataDict` (`Dictionary<Type, object>` 타입)에 데이터 타입별로 캐싱되어, 게임 내 다른 시스템에서 사용됩니다.
    * `ItemDataManager`는 `DataManager.AllDataDict`에서 아이템 관련 타입의 데이터를 가져와 관리합니다.
<p align="center">
  <strong>&lt;아이템 클래스의 데이터 캐싱&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/3a36e9ed-a833-4e30-b9ac-41880a50a860" alt="데이터 캐싱" width="70%"/>
</div>
<br>


5.  **로컬 데이터 활용**:
    * Google 스프레드시트 접근 불가 시, `LoadAllDataFromLocal()` 메서드가 로컬에 JSON 파일로 저장된 데이터를 로드합니다.
    * 스프레드시트에서 새 데이터를 가져오면, `SaveDataToFile()` 메서드가 기존 로컬 데이터와 비교 후 변경된 경우 최신 데이터로 덮어씁니다. `BinaryCheck<T>()`가 데이터 변경 여부를 확인합니다.
<p align="center">
  <strong>&lt;데이터 변경 확인(바이너리비교)&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/56236faf-6a3d-4658-bc5e-fdad2c12e310" alt="데이터 변경확인" width="30%"/>
</div>
<br>

---



### 🏠 로비 (Lobby)

> 플레이어는 계정 인증 후 **로비 화면**으로 이동하여, 다른 플레이어와 소통하고 함께 게임을 즐길 **방을 찾거나 만들 수 있습니다.**

<br/>

<p align="center">
  <strong>&lt;로비화면&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/bd999c4a-0074-49a7-a73b-a84fe01a6a85" alt="로비화면" width="70%"/>
</div>

<br/>

#### 🚪 로비 입장 및 준비 과정

게임에 접속하면 가장 먼저 로비로 입장하기 위한 준비를 시작합니다.

* **서비스 연결 및 인증**: Unity에서 제공하는 온라인 서비스에 연결하고, 플레이어마다 고유한 ID를 받아옵니다.
* **중복 접속 확인**: 혹시 이미 다른 곳에서 같은 계정으로 접속 중인지 확인하여, 중복 접속을 막습니다.
* **대기 로비 참가**: 모든 준비가 끝나면, 다른 플레이어들과 함께 머무르며 방을 탐색하거나 생성할 수 있는 '대기 로비' 공간에 자동으로 들어가게 됩니다. 만약 아무도 없는 첫 접속이라면, 새로운 대기 로비가 만들어집니다.

<br/>

#### 💬 플레이어 간 소통 (채팅)

로비에서는 다른 플레이어들과 실시간으로 대화할 수 있는 채팅 기능이 제공됩니다.

* Vivox 서비스를 이용하여 텍스트 채팅을 지원합니다.
* 이를 통해 함께 게임 할 파티원을 구하거나, 게임에 대한 정보를 나누는 등 다양한 상호작용이 가능합니다.

<p align="center">
  <img src="https://github.com/user-attachments/assets/f3e38ca6-cecf-412b-8071-312cc87864b6" alt="로비 채팅" width="80%"/>
  <br/>
  <sub><strong>&lt;로비 채팅창&gt;</strong><br/>다른 플레이어와 텍스트로 대화할 수 있습니다. (Vivox 연동)</sub>
</p>

<br/>

#### 🔍 게임 방 탐색 및 참가

다른 플레이어가 만들어 놓은 게임 방을 찾아 참여할 수 있습니다.

* 현재 생성되어 있는 공개 게임 방들의 목록이 실시간으로 표시됩니다.
* 새로고침 버튼을 통해 언제든지 최신 방 목록을 불러올 수 있습니다.
* 목록에서 원하는 방을 선택하고 '참가' 버튼을 누르면 해당 방으로 입장합니다.
* 만약 선택한 방에 비밀번호가 설정되어 있다면, 올바른 비밀번호를 입력해야만 들어갈 수 있습니다.

<br/>

<table style="width:100%; border:0;">
  <tr>
    <td align="center" valign="top" style="width:50%;">
      <figure style="margin:0;">
        <img src="https://github.com/user-attachments/assets/50ed75db-9775-49b1-ab05-3fce2d648a9d" alt="방 목록" height="300">
        <figcaption>
          <br/>
          <strong>&lt;게임 방 목록&gt;</strong><br>
          현재 참여 가능한 방들의 이름,
           <br/>
          인원수 등의 정보를 보여줍니다.
        </figcaption>
      </figure>
    </td>
    <td align="center" valign="top" style="width:50%;">
      <figure style="margin:0;">
        <img src="https://github.com/user-attachments/assets/7ef4241f-fe63-49e4-8675-91a6ca7e26af" alt="비밀번호 입력" height="300">
        <figcaption>
          <br/>
          <strong>&lt;비밀번호 입력창&gt;</strong><br>
          비공개 방에 참여하기 위해 비밀번호를 입력하는 화면입니다.
        </figcaption>
      </figure>
    </td>
  </tr>
</table>

<br/>

#### ➕ 게임 방 생성

원한다면 직접 새로운 게임 방을 만들 수도 있습니다.

* 방 만들기 화면에서 만들고 싶은 방의 이름과 최대 참가 가능 인원 수를 설정합니다.
* 다른 플레이어들이 함부로 들어오지 못하도록 비밀번호를 설정할 수도 있습니다.
* 설정을 완료하고 방을 만들면, 이 방은 다른 플레이어들의 방 목록에도 나타나 함께 플레이할 팀원을 모을 수 있습니다.

<p align="center">
  <img src="https://github.com/user-attachments/assets/adf8bd6f-3f22-4dff-89ba-5fbac08a2c82" alt="방 생성" width="30%"/>
  <br/>
  <sub><strong>&lt;게임 방 생성 설정&gt;</strong><br/>새로운 방의 이름, 최대 인원, 비밀번호 등을 설정합니다.</sub>
</p>

<br/>

#### ⚔️ 캐릭터 선택 및 게임 준비

성공적으로 게임 방에 들어가면, 플레이어는 자신이 플레이할 캐릭터를 선택하고 게임 시작을 준비합니다.

* 다양한 클래스 중 원하는 캐릭터를 선택합니다.
* 같은 방에 있는 다른 플레이어들이 어떤 캐릭터를 골랐는지, 게임을 시작할 준비가 되었는지 실시간으로 확인할 수 있습니다.
* 모든 플레이어가 "준비 완료" 상태가 되면, 방을 만든 방장이 게임을 시작할 수 있습니다.

<p align="center">
  <img src="https://github.com/user-attachments/assets/494afb30-753a-4553-bd4f-158943e5877e" alt="캐릭터 선택" width="80%"/>
  <br/>
  <sub><strong>&lt;캐릭터 선택 및 준비 완료&gt;</strong><br/>방에 참가한 플레이어들이 각자 플레이할 캐릭터를 고르고 "준비" 상태를 표시합니다.</sub>
</p>

<br/>

**로비 시스템의 안정성 유지**:

* 방을 만든 플레이어(호스트)는 방이 갑자기 사라지지 않도록 주기적으로 서버에 "방이 아직 살아있음!"이라는 신호(하트비트)를 보냅니다.
* 플레이어가 방에 새로 들어오거나 나가는 등의 변화는 즉시 모든 참가자에게 알려져 화면이 업데이트됩니다.
  
---

<br/>

### 🔗 릴레이 서버 (Relay Server)

> 플레이어 간 직접적인 P2P 연결의 어려움을 해결하고 안정적인 멀티플레이 환경을 제공하기 위해 Unity Relay 서비스를 사용합니다. 이를 통해 별도의 서버 구축 없이도 원활한 게임 연결을 지원합니다.

#### 🛠️ 주요 구현 내용

* **호스트 마이그레이션 (Host Migration)**:
    * 로비 시스템과 연동하여, 기존 호스트가 게임에서 나가면 새로운 호스트가 릴레이 서버의 할당(Allocation) 정보를 이어받아 게임 세션을 계속 유지할 수 있도록 설계되었습니다.
    * 이는 로비 매니저에서 호스트 변경 이벤트를 감지하고, 새로운 호스트에게 릴레이 서버 재설정 권한을 부여하는 방식으로 처리됩니다.
      
<p align="center">
  <img src="https://github.com/user-attachments/assets/ba3a59e4-f8af-4adb-94a1-db202a7630c2" alt="캐릭터 선택" width="80%"/>
  <br/>
  <sub><strong>&lt;호스트 이전(호스트 마이그레이션)&gt;</strong><br/>호스트가 방을 떠나면 다른 플레이어가 호스트를 위임 받습니다.</sub>
</p>

* **릴레이 데이터와 로비 데이터 연동**:
    * 호스트가 릴레이 서버에 성공적으로 방을 할당받으면, 생성된 참여 코드(Join Code)는 로비 데이터의 일부로 저장됩니다.
    * 이를 통해 다른 클라이언트들이 로비에서 방 정보를 보고, 해당 참여 코드를 사용하여 릴레이 서버에 접속할 수 있도록 합니다.
      
<br/>

---
### 🚀 문제 해결 및 기술 개선 사례

프로젝트를 진행하면서 다양한 기술적 문제에 직면했으며, 이를 해결하고 시스템을 개선하기 위해 다음과 같은 노력을 기울였습니다.

<br/>

#### 1. 상태 관리의 유연성 확보: 유한 상태 머신에서 전략 패턴으로

* **🤔 문제점:**
    * 초기 플레이어 캐릭터의 상태(이동, 공격, 정지 등)를 유한 상태 머신(FSM) 방식으로 구현했으나, 새로운 상태를 추가하거나 기존 상태의 로직을 변경할 때 코드 수정 범위가 넓어지고 복잡도가 증가했습니다. 특히, 각 상태에 따른 애니메이션 전환 로직이 강하게 결합되어 유지보수가 어려웠습니다.

* **💡 해결 과정:**
    * 상태 패턴(State Pattern)과 유사한 전략 패턴(Strategy Pattern)을 도입하여 각 상태를 독립적인 클래스(`IState` 인터페이스를 구현하는 형태)로 분리했습니다.
    * `BaseController` 클래스는 현재 상태 객체(`CurrentStateType`)를 통해 해당 상태의 로직을 실행하고, `StateAnimationDict`를 통해 상태 변경 시 적절한 애니메이션을 호출하도록 설계했습니다.
    * 이를 통해 각 상태의 행동 로직과 애니메이션 전환 로직을 캡슐화하고, 새로운 상태 추가 시 기존 코드에 미치는 영향을 최소화했습니다.

* **✨ 개선 결과:**
    * 코드의 가독성과 확장성이 크게 향상되었습니다. 새로운 플레이어 스킬이나 행동 상태를 추가할 때, `IState`를 구현하는 새 클래스를 만들고 `BaseController`에 등록하는 것만으로 확장이 가능해졌습니다.
    * 각 상태 로직이 분리되어 테스트와 디버깅이 용이해졌습니다.

<br/>

#### 2. 로비 콜백 안정성 확보: Unity Multiplayer 팀과의 협업

* **🤔 문제점:**
    * Unity Lobby 서비스 사용 중, 플레이어 입장/퇴장 및 로비 데이터 변경에 대한 콜백 함수가 간헐적으로 호출되지 않거나 지연되는 문제가 발생했습니다. 이로 인해 로비 UI가 실시간으로 업데이트되지 않아 사용자 경험을 저해했습니다.

* **💡 해결 과정:**
    * 문제 재현을 위한 테스트 케이스를 작성하고, Unity 프로파일러 및 네트워크 로그를 분석하여 문제 상황을 구체화했습니다.
    * 분석 내용을 바탕으로 Unity Multiplayer 팀에 공식적으로 문의하였고, 관련 정보를 제공하며 문제 해결을 위해 협력했습니다.
    * Unity 팀의 피드백과 패치를 통해 콜백 시스템의 안정성을 확보할 수 있었습니다.

* **✨ 개선 결과:**
    * 로비 콜백이 안정적으로 수신되어, 플레이어 목록, 방 상태 등의 정보가 UI에 실시간으로 정확하게 반영되게 되었습니다.
    * 로비 시스템의 전반적인 안정성이 향상되어 사용자 경험이 개선되었습니다.

<br/>

#### 3. 네트워크 파티클 최적화: GC 발생 최소화를 위한 네트워크 오브젝트 풀링

* **🤔 문제점:**
    * 보스 스킬이나 플레이어 스킬 사용 시 다수의 파티클 이펙트가 네트워크 상에서 빈번하게 생성 및 소멸되었습니다. 이로 인해 가비지 컬렉션(GC)이 자주 발생하여 프레임 드랍(끊김 현상)을 유발했고, 특히 네트워크 환경에서는 이러한 오브젝트들의 스폰/디스폰 오버헤드가 컸습니다.

* **💡 해결 과정:**
    * 자주 사용되는 네트워크 파티클 프리팹들을 미리 일정 개수만큼 생성해두고 재사용하는 **네트워크 오브젝트 풀링(Network Object Pooling)** 시스템을 `NGO_PoolManager`를 통해 구현했습니다.
    * 파티클이 필요할 때 풀에서 가져와 사용하고, 사용이 끝나면 풀에 반납하여 GC 발생을 최소화했습니다.
    * 또한, `ISpawnBehavior` 인터페이스와 `SpawnParamBase` 구조체를 활용하여 다수의 파티클 위치 및 설정 값을 단일 RPC로 전송하고, 각 클라이언트에서 이를 기반으로 로컬 파티클을 효율적으로 생성/관리하도록 개선했습니다.

* **✨ 개선 결과:**
    * 파티클 관련 GC 발생 빈도가 현저히 감소하여 게임의 전반적인 프레임 안정성이 향상되었습니다.
    * 네트워크 스폰/디스폰 관련 RPC 호출 수가 줄어들어 네트워크 부하가 감소했고, 반응 속도가 개선되었습니다.

<br/>

#### 4. 애니메이션 속도 동기화: 예측과 보간을 통한 부드러운 움직임 구현

* **🤔 문제점:**
    * 네트워크 지연으로 인해 서버(호스트)와 클라이언트 간 캐릭터 애니메이션 속도에 차이가 발생하여, 특정 애니메이션(예: 보스 공격 애니메이션의 감속 효과)이 부자연스럽게 보이거나 타이밍이 맞지 않는 문제가 있었습니다.

* **💡 해결 과정:**
    * 서버(호스트)는 주요 애니메이션의 시작 시점, 길이, 감속 비율, 정지 임계값 등의 정보를 `CurrentAnimInfo` 구조체에 담아 모든 클라이언트에게 RPC로 전송합니다.
    * 각 클라이언트는 수신한 정보를 바탕으로 애니메이션의 진행 상태를 로컬에서 예측하고, 서버 시간(`NetworkManager.Singleton.ServerTime.Time`)을 기준으로 경과 시간을 계산하여 애니메이션 속도를 점진적으로 보간(Lerp)합니다.
    * `BossController`의 `TryGetAnimationSpeed` 메서드에서 이러한 로직을 처리하여, 지정된 감속 비율에 따라 애니메이션 속도가 부드럽게 변화하도록 했습니다.

* **✨ 개선 결과:**
    * 네트워크 지연이 있는 상황에서도 클라이언트에서 캐릭터 애니메이션이 훨씬 부드럽고 자연스럽게 재생됩니다.
    * 공격 애니메이션과 실제 타격 판정 사이의 동기화 정확도가 향상되어 게임 플레이 경험이 개선되었습니다.

<br/>

#### 5. 보스 AI 확장성 개선: 유한 상태 머신에서 비헤이비어 트리로 전환

* **🤔 문제점:**
    * 초기 보스 몬스터의 AI를 플레이어와 마찬가지로 유한 상태 머신(FSM)으로 구현했으나, 보스의 행동 패턴이 다양해지고 복잡해짐에 따라 상태 추가 및 전환 로직 관리가 어려워졌습니다. FSM은 복잡한 조건 분기나 병렬적인 행동 표현에 한계가 있었습니다.

* **💡 해결 과정:**
    * 보스 AI 구현을 위해 **비헤이비어 트리(Behavior Tree)**를 도입했습니다. Opsive의 Behavior Designer 에셋을 활용하여 보스의 다양한 행동(이동, 기본 공격, 스킬 사용, 특정 조건에 따른 패턴 변화 등)을 모듈화된 노드 형태로 설계했습니다.
    * 비헤이비어 트리를 통해 조건 확인(`CheckBossHpBelowCondition`, `CheckAttackProbability`), 행동 실행(`BossAttack`, `BossSkill1`, `BossSkill2`), 흐름 제어(시퀀스, 셀렉터 등)를 직관적으로 구성할 수 있게 되었습니다.
    * 각 행동 노드는 `BossGolemController` 및 `BossGolemNetworkController`와 상호작용하여 애니메이션, 네트워크 동기화 등을 처리합니다.

* **✨ 개선 결과:**
    * 보스 AI의 복잡한 행동 패턴을 보다 체계적이고 시각적으로 관리할 수 있게 되었습니다.
    * 새로운 스킬이나 행동 패턴을 추가하거나 기존 패턴을 수정하는 작업이 훨씬 용이해졌으며, 다양한 조건에 따른 AI 반응을 쉽게 구현할 수 있게 되어 보스전의 깊이가 더해졌습니다.
    * AI 로직과 실제 행동 실행 코드가 분리되어 가독성과 유지보수성이 향상되었습니다.

<br/>

#### 6. 테스트 용이성 확보: 컴포넌트 패턴에서 의존성 주입(DI) 패턴으로의 전환

* **🤔 문제점:**
    * 초기 테스트 코드 작성 시, 테스트코드를 작성해 본래코드와 같은위치의 컴포넌트로 만들어 테스트코드를 넣고 본코드를 비활성화 하는식의 컴포넌트 기반의 테스트 방식을 사용했습니다. 이로 인해 테스트 대상 클래스가 특정 씬 구성이나 다른 컴포넌트의 존재에 강하게 의존하게 되어, 단위 테스트(Unit Test) 작성 및 실행이 어렵고, 테스트 환경 설정이 복잡했습니다.

* **💡 해결 과정:**
    * 테스트 대상 클래스가 필요로 하는 의존성을 외부에서 주입받는 **의존성 주입(Dependency Injection, DI)** 패턴을 점진적으로 도입했습니다.
    * 예를 들어, 특정 씬(`PlaySceneMockUnitTest`, `BattleSceneMockUnitTest` 등)의 테스트 설정 시, 필요한 매니저나 서비스의 모의(Mock) 객체를 생성하여 주입하거나, 테스트용 초기화 메서드를 통해 의존성을 설정하도록 변경했습니다.
    * `ISceneSpawnBehaviour`와 같은 인터페이스를 활용하여, 실제 씬 로직과 테스트용 씬 로직을 분리하고, 생성자나 초기화 메서드를 통해 필요한 의존 객체(예: `UI_Loading`)를 전달받도록 구조를 변경했습니다.

* **✨ 개선 결과:**
    * 클래스 간의 결합도가 낮아져 각 컴포넌트를 독립적으로 테스트하기 용이해졌습니다.
    * 모의 객체를 활용하여 외부 환경에 구애받지 않고 단위 테스트를 수행할 수 있게 되어 테스트 커버리지를 높이고 버그를 조기에 발견하는 데 도움이 되었습니다.
    * 코드의 재사용성과 유연성이 향상되었습니다.

---


![Boss Flow](https://github.com/user-attachments/assets/33e41408-493a-4778-830d-c0c69d4055a5)
