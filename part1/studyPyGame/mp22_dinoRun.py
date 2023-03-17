# dinoRun
import pygame
import os
import random

pygame.init()

ASSETS = './studyPyGame/Assets/'
SCREEN_WIDTH = 1100    # 게임 윈도우 넓이 
SCREEN = pygame.display.set_mode((1100, 600))
icon = pygame.image.load('./studyPyGame/dinoRun.png')
pygame.display.set_icon(icon)
# 배경이미지 로드 
BG = pygame.image.load(os.path.join(f'{ASSETS}Other', 'Track.png'))

RUNNING = [pygame.image.load(f'{ASSETS}Dino/DinoRun1.png'), 
           pygame.image.load(f'{ASSETS}Dino/DinoRun2.png')]
DUCKING =  [pygame.image.load(f'{ASSETS}Dino/DinoDuck1.png'), 
           pygame.image.load(f'{ASSETS}Dino/DinoDuck2.png')]
JUMPING = pygame.image.load(f'{ASSETS}Dino/DinoJump.png')

# 구름 이미지 
CLOUD = pygame.image.load(f'{ASSETS}Other/Cloud.png')

# 익명 이미지 로드 
BIRD = [pygame.image.load(f'{ASSETS}Bird/Bird1.png'), 
        pygame.image.load(f'{ASSETS}Bird/Bird2.png')]

# 선인장 이미지 로드 
LARGE_CACTUS = [pygame.image.load(f'{ASSETS}Cactus/LargeCactus1.png'), 
                pygame.image.load(f'{ASSETS}Cactus/LargeCactus2.png'),
                pygame.image.load(f'{ASSETS}Cactus/LargeCactus3.png')]

SMALL_CACTUS = [pygame.image.load(f'{ASSETS}Cactus/SmallCactus1.png'), 
                pygame.image.load(f'{ASSETS}Cactus/SmallCactus2.png'),
                pygame.image.load(f'{ASSETS}Cactus/SmallCactus3.png')]

class Dino: # 공룡 클래스
    X_POS = 80; Y_POS = 310; Y_POS_DUCK = 340; JUMP_VEL = 9.0

    def __init__(self) -> None:
        self.run_img = RUNNING; self.duck_img = DUCKING; self.jump_img = JUMPING
        self.dino_run = True; self.dino_duck = False; self.dino_jump = False

        self.step_index = 0
        self.jump_vel = self.JUMP_VEL # 점프 초기값 9.0
        self.image = self.run_img[0]
        self.dino_rect = self.image.get_rect() # 이미지 사각형 정보
        self.dino_rect.x = self.X_POS
        self.dino_rect.y = self.Y_POS

    def update(self, userInput) -> None:
        if self.dino_run:
            self.run()
        elif self.dino_duck:
            self.duck()
        elif self.dino_jump:
            self.jump()
        
        if self.step_index >= 10: self.step_index = 0 # 애니메이션 스텝

        if userInput[pygame.K_UP] and not self.dino_jump: # 점프
            self.dino_run = False
            self.dino_duck = False
            self.dino_jump = True
            self.dino_rect.y = self.Y_POS
        elif userInput[pygame.K_DOWN] and not self.dino_jump: # 수구리
            self.dino_run = False
            self.dino_duck = True
            self.dino_jump = False
        elif not(self.dino_jump or userInput[pygame.K_DOWN]): # 런
            self.dino_run = True
            self.dino_duck = False
            self.dino_jump = False      

    def run(self):
        self.image = self.run_img[self.step_index // 5] 
        self.dino_rect = self.image.get_rect() # 이미지 사각형 정보
        self.dino_rect.x = self.X_POS
        self.dino_rect.y = self.Y_POS
        self.step_index += 1

    def duck(self):
        self.image = self.duck_img[self.step_index // 5] # duck_img
        self.dino_rect = self.image.get_rect() # 이미지 사각형 정보
        self.dino_rect.x = self.X_POS
        self.dino_rect.y = self.Y_POS_DUCK # 이미지 높이가 작으니까
        self.step_index += 1  

    def jump(self):
        self.image = self.jump_img
        if self.dino_jump:
            self.dino_rect.y -= self.jump_vel * 4
            self.jump_vel -= 0.8
        if self.jump_vel < -self.JUMP_VEL: # -9.0이 되면 점프중단
            self.dino_jump = False
            self.jump_vel = self.JUMP_VEL # 9.0으로 초기화

    def draw(self, SCREEN) -> None:
        SCREEN.blit(self.image, (self.dino_rect.x, self.dino_rect.y))

class Cloud: # 구름 클래스 
    def __init__(self) -> None:
        self.x = SCREEN_WIDTH + random.randint(300, 500)
        self.y = random.randint(50, 100)     # y 50 에서 100 사이에만 구름을 만들겠음
        self.image = CLOUD
        self.width = self.image.get_width()

    def update(self) -> None:
        self.x -= game_speed
        if self.x < -self.width:            # 화면 밖으로 벗어나면 
            self.x = SCREEN_WIDTH + random.randint(1300, 2000)
            self.y = random.randint(50, 100)

    def draw(self, SCREEN) -> None:
        SCREEN.blit(self.image, (self.x, self.y))

class Obstacle:   # 장애물 클래스 
    def __init__(self, image, type) -> None:
        self.image = image
        self.type = type
        self.rect = self.image[self.type].get_rect()
        self.rect.x = SCREEN_WIDTH     # 1100 부터 

    def update(self) -> None:
        self.rect.x -= game_speed
        if self.rect.x <= -self.rect.width:    # 왼쪽 화면 밖으로 벗어나면 
            obstacles.pop()        # 장애물 리스트에서 하나 꺼내오기

    def draw(self, SCREEN)-> None:
        SCREEN.blit(self.image[self.type], self.rect)

class Bird(Obstacle):    # 장애물 (Obstacle 상속 클래스) 상속 클래스
    def __init__(self, image) -> None:
        self.type = 0    # 새는 0
        super().__init__(image, self.type)
        self.rect.y = 250 # 새는 땅에 있음 안되니까..
        self.index = 0   # 새는 이미지가 2개라서 0번 이미지로 시작한다는 

    def draw(self, SCREEN) -> None:    # draw 재정의 (객체지향을 생각해)
        if self.index >= 9:
            self.index = 0
        SCREEN.blit(self.image[self.index // 5], self.rect)
        self.index += 1

class LargeCactus(Obstacle):
    def __init__(self, image) -> None:
        self.type = random.randint(0, 2)    # 큰 선인장 세개니까 하나 고름 
        super().__init__(image, self.type)
        self.rect.y = 300

class SmallCactus(Obstacle):
    def __init__(self, image) -> None:
        self.type = random.randint(0, 2)  # 작은 선인장 세개니까 그 중 하나 고름 
        super().__init__(image, self.type)
        self.rect.y = 325

def main():
    global game_speed, x_pos_bg, y_pos_bg, points, obstacles
    x_pos_bg = 0
    y_pos_bg = 380
    points = 0
    run = True
    clock = pygame.time.Clock()
    dino = Dino()          # 공룡 객체 
    cloud = Cloud()
    game_speed = 14
    obstacles = []
    
    font = pygame.font.Font(f'{ASSETS}NanumGothicBold.ttf', 20)   # 나중에 나눔고딕으로 변경

    def score():    # 함수 내 함수 (점수 표시)
        global points, game_speed
        points += 1
        if points % 100 == 0:   # 점수가 100 단위로 증가시 
            game_speed += 1     # 게임 속도를 증가 

        txtScore = font.render(f'SCORE: {points}', True, (83,83,83))
        txrRect = txtScore.get_rect()
        txrRect.center = (1000, 40)
        SCREEN.blit(txtScore, txrRect)

    # 함수 내 함수 (배경 표시)
    def background():             # 땅바닥을 uadate, draw를 동시에
        global x_pos_bg, y_pos_bg
        image_width = BG.get_width()           # 2404가 나옴 
        SCREEN.blit(BG, (x_pos_bg, y_pos_bg))  # 0, 380 먼저 그림 
        SCREEN.blit(BG, (image_width+x_pos_bg, y_pos_bg))    # 2404+0, 380
        if x_pos_bg <= -image_width:
            x_pos_bg = 0
        
        x_pos_bg-=game_speed

    while run:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                run = False

        SCREEN.fill((255,255,255)) # 배경 흰색
        userInput = pygame.key.get_pressed()
        
        background()
        score()

        cloud.draw(SCREEN)     # 구름 애니매이션 
        cloud.update()           # 구름이 공룡보다 먼저 그려져야함 (배경)

        dino.draw(SCREEN)      # 공룡 그리기
        dino.update(userInput) 

        if len(obstacles)==0:
            if random.randint(0, 2)==0:    # 작은 선인장
                obstacles.append(SmallCactus(SMALL_CACTUS))
            elif random.randint(0, 2)==1:  # 큰 선인장
                obstacles.append(LargeCactus(LARGE_CACTUS))
            elif random.randint(0, 2) ==2: # to
                obstacles.append(Bird(BIRD))

        for obs in obstacles:
            obs.draw(SCREEN)
            obs.update()
            if dino.dino_rect.colliderect(obs.rect):   # 물체 충돌 감지 (총알 맞거나 장애물 부딪히는거)
                pygame.draw.rect(SCREEN, (255, 0, 0), dino.dino_rect, 3)

        clock.tick(30)
        pygame.display.update() # 초당 30번 update 수행

if __name__ == '__main__':
    main()