// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { ProductosComponent } from './features/productos/productos.component';
import {TransaccionesComponent} from './features/transacciones/transacciones.component';
export const routes: Routes = [
    {
        path: 'productos',
        component: ProductosComponent,
    },
     { path: 'transacciones', component: TransaccionesComponent },
    {
        path: '',
        redirectTo: 'productos',
        pathMatch: 'full'
    }
];
