# 스레드 미사용 앱
import sys
from PyQt5 import uic
from PyQt5.QtWidgets import *
from PyQt5.QtGui import *     # QIcon은 여기 
from PyQt5.QtCore import *    # Qt.white..

class qtApp(QWidget):

    def __init__(self):
        super().__init__()
        uic.loadUi('./studyThread/threadApp.ui', self)
        self.setWindowTitle('노 스레드 앱')

if __name__ == '__main__' :
    app = QApplication(sys.argv)
    ex = qtApp()     
    ex.show()
    sys.exit(app.exec_())   