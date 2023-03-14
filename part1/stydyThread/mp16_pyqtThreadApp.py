# 스레드 사용 앱
import sys
from PyQt5 import uic
from PyQt5.QtWidgets import *
from PyQt5.QtGui import *     # QIcon은 여기 
from PyQt5.QtCore import *    # Qt.white..

import time

class BackgroundWorker(QThread):     # PyQt5 스레드를 위한 클래스 존재 
    procChanged = pyqtSignal(str)

    def __init__(self, count=0 ,parent = None) -> None:
        super().__init__(parent)
        self.parent = parent
        self.working = True     # 스레드 동작 여부 
        self.count = count

    def run(self):
        # self.parent.pgdTask.setRange(0, 100)
        # for i in range(0, 101):
        #     print(f'스레드 출력 > {i}')
        #     self.parent.pgdTask.setValue(i)
        #     self.parent.txtLog.append(f'스레드 출력 > {i}')
        # 스레드 화면 오류 -> 못씀
        while self.working:
            self.procChanged.emit(self.count)     # 시그널을 내보냄 
            self.count += 1     # 값 증가만 
            time.sleep(0.0001)
class qtApp(QWidget):

    def __init__(self):
        super().__init__()
        uic.loadUi('./studyThread/threadApp.ui', self)
        self.setWindowTitle('스레드 앱')
        self.pgdTask.setValue(0)

        self.btnStart.clicked.connect(self.btnStartClicked)
        # 스레드 초기화 
        self.worker = BackgroundWorker(parent=self, count=0)
        # 백그라운드 워커에 있는 시그널 접근 슬롯 함수 
        self.worker.procChanged.connect(self.proUpdated)
        # 초기화 
        self.pgdTask.setRange(0, 1000000)

    @pyqtSlot(int)
    def proUpdated(self, count):
        self.txtLog.append(f'스레드 출력 > {count}')
        self.pgdTask.setValue(count)
        print(f'스레드 출력 > {count}')
        
    @pyqtSlot()
    def btnStartClicked(self):
        self.worker.start()
        self.worker.working = True
        # th = BackgroundWorker(self)
        # th.start()

if __name__ == '__main__' :
    app = QApplication(sys.argv)
    ex = qtApp()     
    ex.show()
    sys.exit(app.exec_())   