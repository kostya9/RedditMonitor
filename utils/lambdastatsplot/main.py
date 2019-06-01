import matplotlib
import matplotlib.pyplot as plt
from matplotlib import rcParams

mem = [128, 256, 512, 768, 1024, 2048]
inserted = [35, 178, 301, 476, 579, 565]
failed = [554, 570, 390, 230, 210, 90]

fig, ax = plt.subplots()

ax.axis([0, 2200, 0, 600])
plt.xticks(mem);

ax.plot(mem, inserted, 'k', label='Успішна обробка')
ax.plot(mem, inserted, 'ko')

ax.plot(mem, failed, color='0.6', label='Помилкова обробка')
ax.plot(mem, failed, color='0.6', marker='o')

ax.legend()


rcParams['axes.titlepad'] = 20 
ax.set(xlabel='Пам\'ять (МБ)', ylabel='Кількість повідомлень',
       title='Продуктивність функції обробки зображень за 5хв')

plt.show()