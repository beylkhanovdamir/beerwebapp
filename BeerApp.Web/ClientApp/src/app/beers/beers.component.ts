
import { fromEvent } from 'rxjs';
import { distinctUntilChanged, debounceTime, map, filter } from 'rxjs/operators';
import { Component, ElementRef, ViewChild, AfterViewInit } from '@angular/core';
import { BeerService } from '../services/beer.service';
import { Beer, BeerStyle } from "../shared/models/beer.model";
import { NgProgress } from 'ngx-progressbar';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'beers',
  templateUrl: './beers.html',
  providers: [BeerService]
})

export class BeersComponent implements AfterViewInit {
  selectedSort = 'abv';
  query = '';
  public selectedStyle: any;
  public filteredBeerList: Beer[] = [];
  public filteredBeerStylesList: BeerStyle[] = [];
  //
  private beerList: Beer[] = [];
  private beerStyles: BeerStyle[] = [];
  private defaultBeerStyle = <BeerStyle>{ id: 0, name: 'All' };
  constructor(private _beerService: BeerService, public ngProgress: NgProgress, private router: Router, private activeRoute: ActivatedRoute) {

  }

  @ViewChild('searchRef', { read: ElementRef }) searchRef: ElementRef | any;

  ngOnInit() {
    const queryParams = this.activeRoute.snapshot.queryParams;

    if (queryParams['q']) {
      this.query = queryParams['q'];
    }
    this.getBeers(this.query);
    this.getBeerStyles();
  }

  ngAfterViewInit() {
    fromEvent((this.searchRef.nativeElement) as any, 'input').pipe(
      map((evt: any) => evt.target.value),
      debounceTime(1100), distinctUntilChanged())
      .subscribe((text: any) => {
        this.getBeers(text);
      });
  }

  getBeers(query?: string) {
    this.ngProgress.start();

    this._beerService.getBeers(query).subscribe((result) => {

      this.beerList = this.filteredBeerList = result || [];
      if (this.query && this.query.length > 0) {
        this.router.navigate(['/'], { queryParams: { q: query } });
        this.filteredBeerStylesList = this.beerStyles.filter(o1 => o1.id === 0 || this.beerList.some(o2 => o2.style != null && o1.id === o2.style.id));
        this.selectedStyle = this.filteredBeerStylesList[0];
      } else {
        this.router.navigate(['/']);
        this.filteredBeerStylesList = this.beerStyles;
        this.selectedStyle = this.filteredBeerStylesList[0];
      }
      this.ngProgress.done();
    }, (error) => {
      this.beerList.length = this.filteredBeerList.length = 0;
      this.ngProgress.done();
      console.log(error)
    }
    );
  }

  getBeersByStyle(style: any) {

    if (!this.query && this.query.length === 0) {
      this.ngProgress.start();
      this._beerService.getBeersByStyle(style.id).subscribe(
        (data) => {
          this.beerList = this.filteredBeerList = data;
          this.ngProgress.done();
        }, (error) => {
          this.ngProgress.done();
          console.log(error)
        }
      );

    } else {
      if (style.id !== this.defaultBeerStyle.id)
        this.filteredBeerList = this.beerList.filter(x => x.style.id === style.id);
      else
        this.filteredBeerList = this.beerList;
    }
  }

  get noResults(): boolean {
    return this.filteredBeerList.length === 0;
  }

  getBeerStyles() {
    this._beerService.getBeerStyles().subscribe((result) => {
      this.beerStyles = result;

      this.beerStyles.splice(0, 0, this.defaultBeerStyle);

      this.filteredBeerStylesList = this.beerStyles;
      this.selectedStyle = this.filteredBeerStylesList[0];
    }, (error) => {
      console.log(error)
    }
    );
  }
}


