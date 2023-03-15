# 이메일 보내기 앱 
import smtplib   # 메일 전송 프로토콜 
from email.mime.text import MIMEText    # Multipurpose Internet Mail Extensions

send_email = 'jangse4233@naver.com'
send_pass = '' # 내 진짜 비밀번호

recv_email = 'jangse4233@gmail.com'

smtp_name = 'smtp.naver.com'
smpt_port = 587  # 포트 번호 

text = '''조심하세요. 빨리 연락주세요. 긴급한 내용입니다.'''

msg = MIMEText(text)
msg['Subject'] = '메일 제목입니다.'
msg['From'] = send_email    # 보내는 이메일
msg['To'] = recv_email      # 받는 메일

print(msg.as_string())

mail = smtplib.SMTP(smtp_name, smpt_port)    # SMTP 객체 생성 
mail.starttls()    # 전송계층 보안 시작 
mail.login(send_email, send_pass)
mail.sendmail(send_email, recv_email, msg=msg.as_string())
mail.quit()
print('전송완료')