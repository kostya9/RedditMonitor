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
    axs.bar(index, buckets)
    axs.set_xlabel('Interval', fontsize=14)
    axs.set_ylabel('Color Frequency', fontsize=14)
    plt.sca(axs)
    plt.xticks(index, labels, fontsize=14, rotation=30)
    plt.title('Distribution of color frequency')

def show_color_plot(axs, values_range, dim):
    X = [[v for v in values_range[i * dim : i * dim + dim]] for i in range(dim)]
    axs.imshow(X, cmap='Reds')


values = [23, 89, 1, 78, 245, 128, 12, 156, 56, 78, 74, 155, 70, 63, 45, 154]

buckets = get_distribution(values)
fix, axs = plt.subplots(2, 1)

show_color_plot(axs[0], values, 4)
show_frequency_plot(axs[1], buckets)

plt.show()