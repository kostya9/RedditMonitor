from PIL import Image
import matplotlib.pyplot as plt
import numpy as np

import sys

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

    pixels = values_range.__len__()

    return [(v / pixels) for v in buckets]

def show_frequency_plot(axs, frequency_buckets, title, color):
    labels = ['0-63', '64-127', '128-191', '192-255']

    index = np.arange(len(labels))
    axs.bar(index, frequency_buckets, color = color, alpha = 0.8)
    plt.ylim(0, 1)
    plt.sca(axs)
    plt.xticks(index, labels)
    plt.title(title)

def show_frequency_plot_stack(axs, frequency_buckets, title, color, frequency_buckets_bottom):
    labels = ['0-63', '64-127', '128-191', '192-255']

    index = np.arange(len(labels))
    axs.bar(index, frequency_buckets, color = color, alpha = 0.8, bottom=frequency_buckets_bottom)
    plt.ylim(0, 1)
    plt.sca(axs)
    plt.xticks(index, labels)
    plt.title(title)

def get_buckets(path):
    im = Image.open(path, 'r')
    pixels = im.getdata()
    red = [set[0] for set in list(pixels)]
    green = [set[1] for set in list(pixels)]
    blue = [set[2] for set in list(pixels)]

    red_buckets = get_distribution(red)
    green_buckets = get_distribution(green)
    blue_buckets = get_distribution(blue)

    return (red_buckets , green_buckets, blue_buckets)

fig, axs = plt.subplots(3, 1)

if len(sys.argv) == 3:
    (red_buckets_left, green_buckets_left, blue_buckets_left) = get_buckets(sys.argv[1])

    (red_buckets_right , green_buckets_right, blue_buckets_right) = get_buckets(sys.argv[2])

    subtracted_red = [abs(x) for x in np.subtract(red_buckets_left, red_buckets_right)]
    subtracted_green = [abs(x) for x in np.subtract(green_buckets_left, green_buckets_right)]
    subtracted_blue = [abs(x) for x in np.subtract(blue_buckets_left, blue_buckets_right)]

    minimum_red = np.minimum(red_buckets_left, red_buckets_right)
    minimum_green = np.minimum(green_buckets_left, green_buckets_right)
    minimum_blue = np.minimum(blue_buckets_left, blue_buckets_right)


    show_frequency_plot(axs[0], minimum_red, 'Червоний', 'lightgrey')
    show_frequency_plot(axs[1], minimum_green, 'Зелений', 'lightgrey')
    show_frequency_plot(axs[2], minimum_blue, 'Синій', 'lightgrey')

    print(subtracted_blue)
    print(subtracted_red)
    print(subtracted_green)

    show_frequency_plot_stack(axs[0], subtracted_red, 'Червоний', 'red', minimum_red)
    show_frequency_plot_stack(axs[1], subtracted_green, 'Зелений', 'red', minimum_green)
    show_frequency_plot_stack(axs[2], subtracted_blue, 'Синій', 'red', minimum_blue)
else:
    (red_buckets, green_buckets, blue_buckets) = get_buckets(sys.argv[1])
    show_frequency_plot(axs[0], red_buckets, 'Червоний', 'lightsteelblue')
    show_frequency_plot(axs[1], green_buckets, 'Зелений', 'lightsteelblue')
    show_frequency_plot(axs[2], blue_buckets, 'Синій', 'lightsteelblue')

fig.tight_layout()


plt.show()