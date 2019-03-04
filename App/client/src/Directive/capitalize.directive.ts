import { Directive, ElementRef, HostListener } from '@angular/core';
@Directive({ selector: '[capitalize]' })
export class CapitalizeDirective {
  constructor(private el: ElementRef) {
    // el.nativeElement.style.backgroundColor = 'yellow';
  }
  // @HostListener('mouseenter') onMouseEnter() { this.highlight('yellow'); }
  // @HostListener('mouseleave') onMouseLeave() { this.highlight(null); }
  @HostListener('blur', ['$event.target'])
  onblur(btn) {
    btn.value=btn.value.toUpperCase()
  }
  // private highlight(color: string) {
  //   this.el.nativeElement.style.backgroundColor = color;
  // }
}