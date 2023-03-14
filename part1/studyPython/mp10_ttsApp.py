# TTS(Text To Speech)
# pip install gTTs
# pip install playsound
from gtts import gTTS
from playsound import playsound

text ='안녕하세요, 장시현 입니다.'

tts = gTTS(text=text, lang='ko')
tts.save('./studyPython/output/hi.mp3')
print('완료!')
playsound('./studyPython/output/hi.mp3')
print('음성출력 완료')