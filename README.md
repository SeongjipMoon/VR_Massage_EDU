# VR_Baby_Massage_Education Simulation

VR_Baby_Massage_Education Simulation

Haptic Feedback을 활용한 VR 베이비 마사지 교육용 콘텐츠입니다.

## 1. 실행을 위해 필요한 것들
Oculus Quest2, Tact Glove

## 2. 실행방법
1. Meta Quest Link를 설치한다.
2. bHaptics Player를 설치한다.
3. Meta Quest Link에 가지고 있는 Oculus Quest2를 연동시킨다.
4. Oculus Quest를 접속하여 Meta Quest Link를 실행하고 있는 컴퓨터에 Rift를 연결시킨다.
5. 블루투스 동글을 컴퓨터에 연결하여, Tactglove_L, Tactglove_R을 연동시킨다.
6. Title.Unity 파일을 열고 실행시킨다.


## 3. 한계
- Tact Glove를 눈높이에 맞추지 않거나, 왼손과 오른손이 겹치게 되서 Oculus Quest2에 가려지게 되면, 제대로 Hand Tracking을 인식하지 못한다.
- Tact Glove로 제공해줄 수 있는 Haptic Feedback은 각 손가락 끝부분과 손목부분밖에 없어서 디테일한 피드백은 구현이 불가능하다. (ex) 손바닥과 손마디부분
- 이번 프로젝트에서는 Haptic Feedback을 주는 과정에서, Feedback 평가방법을 제대로 구현하지 못해서, Haptic Feedback을 제공해주지만, 
제대로 수행하고 있는지, 평가를 해주는 기능을 포함하지 않고 있다.
