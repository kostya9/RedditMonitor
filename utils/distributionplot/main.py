import matplotlib.pyplot as plt

import numpy as np


def get_distribution(values_range):
    min_value = 0
    max_value = 255

    interval_count = 4

    step = (max_value - min_value + 1) / interval_count

    buckets = [0 for i in range(interval_count)]

    def get_index(val):
        return int((val - min_value) / step)
    
    for v in values_range:
        buckets[get_index(v)] += 1

    return buckets

def show_frequency_plot(axs, frequency_buckets):
    labels = ['0-63', '64-127', '128-191', '192-255']

    index = np.arange(len(labels))
    axs.bar(index, frequency_buckets, color = '0.75')
    axs.set_xlabel('Інтервали')
    axs.set_ylabel('Кількість пікселів')
    plt.sca(axs)
    plt.xticks(index, labels)
    plt.title('Розподіл червоного кольору')

def show_normalized_frequency_plot(axs, frequency_buckets):
    labels = ['0-63', '64-127', '128-191', '192-255']

    index = np.arange(len(labels))
    axs.bar(index, frequency_buckets, color = '0.75')
    axs.set_xlabel('Інтервали')
    axs.set_ylabel('Частота')
    plt.sca(axs)
    plt.xticks(index, labels)
    plt.title('Нормалізований розподіл червоного кольору')

def show_color_plot(axs, values_range, dim):
    X = [[v for v in values_range[i * dim : i * dim + dim]] for i in range(dim)]


    axs.imshow(X, cmap='Greys')
    axs.axis('off')

    axs.spines['left'].set_color('c')
    axs.spines['right'].set_visible(True)
    axs.spines['top'].set_linewidth(5)
    axs.spines['left'].set_linewidth(5)


values = [45, 89, 156, 78, 245, 19, 12, 128, 56, 78, 74, 155, 70, 63, 45, 154]

buckets = get_distribution(values)

fig, axs = plt.subplots(1, 1)

show_color_plot(axs, values, 4)


fig, axs = plt.subplots(1, 2)
fig.tight_layout()

show_frequency_plot(axs[0], buckets)

pixels = values.__len__()
normalized = [(v / pixels) for v in buckets]
show_normalized_frequency_plot(axs[1], normalized)

plt.show()