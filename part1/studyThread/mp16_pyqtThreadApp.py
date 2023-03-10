# 스레드 사용 앱
import sys
from PyQt5 import uic
from PyQt5.QtWidgets import *
from PyQt5.QtGui import *     # QIcon은 여기 
from PyQt5.QtCore import *    # Qt.white..
import time

MAX = 1000

class BackgroundWorker(QThread):     # PyQt5 스레드를 위한 클래스 존재 
    procChanged = pyqtSignal(int)    # 커스텀 시그널 (이걸 해야지 스레드 처리 가능)// 마우스 클릭같은 시그널을 따로 만드는 것 

    def __init__(self, count=0, parent = None) -> None:
        super().__init__(parent)
        self.main = parent
        self.working = False     # 스레드 동작 여부 
        self.count = count

    def run(self):             # 어떤 스레드를 start 하면 일어나는 함수 // thread.start() --> run()  //  대신 실행 
        # self.parent.pgdTask.setRange(0, 100)
        # for i in range(0, 101):
        #     print(f'스레드 출력 > {i}')
        #     self.parent.pgdTask.setValue(i)
        #     self.parent.txtLog.append(f'스레드 출력 > {i}')
        # 스레드 화면 오류 -> 못씀
        while self.working:
            if self.count <= MAX :
                self.procChanged.emit(self.count)     # emit : 시그널을 내보냄 // 뭘하든 무조건 내보내는 함수임 --> 값을 계속 전달하는 함수 
                self.count += 1     # 값 증가만 // 업무 프로세스가 동작하는 위치 
                time.sleep(0.0001)  # 시간을 잠시 멈추는 // 스레드의 동시성 때문에 시간을 쪼개줘야함 안쪼개주면 gui랑 번갈아 가면서 일처리를 x 
            # 너무 세밀하게 시간을 주면 gui를 처리를 못함 
class qtApp(QWidget):

    def __init__(self):
        super().__init__()
        uic.loadUi('./studyThread/threadApp.ui', self)
        self.setWindowTitle('스레드 앱')
        self.pgdTask.setValue(0)
        # 내장된 시그널 (기본적으로 있는거 )
        self.btnStart.clicked.connect(self.btnStartClicked)
        # 스레드 생성
        self.worker = BackgroundWorker(parent=self, count=0)
        # 백그라운드 워커에 있는 시그널 접근 슬롯 함수 
        self.worker.procChanged.connect(self.proUpdated)
        # 초기화 
        self.pgdTask.setRange(0, 1000000)

    #@pyqtSlot(int)   # 데코레이셤 -> 없어도 동작함 
    def proUpdated(self, count):
        self.txtLog.append(f'스레드 출력 > {count}')
        self.pgdTask.setValue(count)
        print(f'스레드 출력 > {count}')
        
    #@pyqtSlot()
    def btnStartClicked(self):
        self.worker.start()          # 스레드 클래스의 run() 함수 실행
        self.worker.working = True
        self.worker.count=0
        # th = BackgroundWorker(self)
        # th.start()

if __name__ == '__main__' :
    app = QApplication(sys.argv)
    ex = qtApp()     
    ex.show()
    sys.exit(app.exec_())   