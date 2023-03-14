# 스레드 학습
# 기본 프로세스 하나 , 서브 스레드 다섯개 동시 진행 

import threading
import time

# thread를 상속 받은 백그라운드 작업 클래스 
class BackgrundWorker(threading.Thread):
    # 생성자 
    def __init__(self, names: str) -> None:
        super().__init__()
        self._name = f'{threading.current_thread().name}: {names}'

    def run(self) -> None:
        print(f'BackgroundWoker start : {self._name}')
        # time.sleep(2)
        print(f'BackgroundWoker end : {self._name}')

if __name__ == '__main__':
    # 기본 프로세스 == 메인 스레드 
    # 프로세스 하나를 스레드라고 (하나의 일을 처리하는 작업 테스크// 명령을 실행하는 작업 영역 ?)
    print('기본 프로세스 시작')    

    for i in range(5):
        name = f'서브 스레드 {i}'
        th = BackgrundWorker(name)
        th.start()     # run이 실행됌 

    print('기본 프로세스 종료')
    
