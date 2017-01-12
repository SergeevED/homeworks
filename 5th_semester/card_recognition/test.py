import csv
import os
import sys

from main import load_samples, classify_cards, extract_cards_orthographic

tests_path = "images/"


def test_one(image_path, verbose=False):
    answers_path = image_path.replace('.jpg', '.txt')
    with open(answers_path, 'rb') as answers_file:
        reader = csv.reader(answers_file)
        parsed_csv = list(reader)
        actual_answers = [item for sublist in parsed_csv for item in sublist]

    cards = extract_cards_orthographic(image_path)
    if cards is None:
        print("Image at path {} not found! Aborted.".format(image_path))
        sys.exit(1)

    samples = load_samples()
    test_answers = classify_cards(cards, samples, verbose=False)
    guessed_ranks = 0
    guessed_suits = 0
    guessed_cards = 0
    total_cards = len(actual_answers)
    for test_answer, actual_answer in zip(test_answers, actual_answers):
        if test_answer[0] == actual_answer[0]:
            guessed_ranks += 1
        if test_answer[1] == actual_answer[1]:
            guessed_suits += 1
        if test_answer == actual_answer:
            guessed_cards += 1

    if verbose:
        print("File: {}".format(image_path))

        print("Expected results: "),
        for actual_answer in actual_answers:
            print("{} ".format(actual_answer)),
        print("")
        print("Received results: "),
        for test_answer in test_answers:
            print("{} ".format(test_answer)),
        print("")

        print("Cards guessed:\t{:2}/{:2}\t{:3.2f}%".format(
            guessed_cards, total_cards,
            100.0 * guessed_cards / total_cards
        ))
        print("Ranks guessed:\t{:2}/{:2}\t{:3.2f}%".format(
            guessed_ranks, total_cards,
            100.0 * guessed_ranks / total_cards
        ))
        print("Suits guessed:\t{:2}/{:2}\t{:3.2f}%".format(
            guessed_suits, total_cards,
            100.0 * guessed_suits / total_cards
        ))

    return guessed_ranks, guessed_suits, guessed_cards, total_cards


def test_all():
    test_image_paths = filter(lambda x: x.endswith(".jpg"), os.listdir(tests_path))
    guessed_ranks = 0
    guessed_suits = 0
    guessed_cards = 0
    total_cards = 0
    for image_filename in test_image_paths:
        image_path = os.path.join(tests_path, image_filename)

        loc_guessed_ranks, loc_guessed_suits, loc_guessed_cards, loc_total_cards = \
            test_one(image_path, verbose=True)

        guessed_ranks += loc_guessed_ranks
        guessed_suits += loc_guessed_suits
        guessed_cards += loc_guessed_cards
        total_cards += loc_total_cards

    print("")
    print("Total:")
    print("Cards guessed:\t{:2}/{:2}\t{:3.2}%".format(guessed_cards, total_cards, 100.0 * guessed_cards / total_cards))
    print("Ranks guessed:\t{:2}/{:2}\t{:3.2}%".format(guessed_ranks, total_cards, 100.0 * guessed_ranks / total_cards))
    print("Suits guessed:\t{:2}/{:2}\t{:3.2}%".format(guessed_suits, total_cards, 100.0 * guessed_suits / total_cards))


if len(sys.argv) < 2:
    test_all()
else:
    test_one(image_path=sys.argv[1], verbose=True)
