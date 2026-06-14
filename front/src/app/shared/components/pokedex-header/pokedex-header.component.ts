import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pokedex-header',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './pokedex-header.component.html',
  styleUrl: './pokedex-header.component.css'
})
export class PokedexHeaderComponent {}
