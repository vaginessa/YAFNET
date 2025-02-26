import { Choice } from '../interfaces/choice';
import { ClassNames } from '../interfaces/class-names';
import { Item } from '../interfaces/item';
import WrappedElement from './wrapped-element';
export default class WrappedSelect extends WrappedElement {
    element: HTMLSelectElement;
    classNames: ClassNames;
    template: (data: object) => HTMLOptionElement;
    constructor({ element, classNames, template, }: {
        element: HTMLSelectElement;
        classNames: ClassNames;
        template: (data: object) => HTMLOptionElement;
    });
    get placeholderOption(): HTMLOptionElement | null;
    get optionGroups(): Element[];
    get options(): Item[] | HTMLOptionElement[];
    set options(options: Item[] | HTMLOptionElement[]);
    optionsAsChoices(): Partial<Choice>[];
    _optionToChoice(option: HTMLOptionElement): Choice;
    _optgroupToChoice(optgroup: HTMLOptGroupElement): Partial<Choice>;
    appendDocFragment(fragment: DocumentFragment): void;
}
//# sourceMappingURL=wrapped-select.d.ts.map