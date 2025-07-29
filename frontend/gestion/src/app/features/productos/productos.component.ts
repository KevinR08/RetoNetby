import { Component } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Producto } from '../../core/models/producto.model';
import { ProductoService } from '../../core/services/producto.service';

/* PrimeNG */
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { ToastModule } from 'primeng/toast';
import { FloatLabelModule } from 'primeng/floatlabel';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-productos',
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.scss'],
  providers: [MessageService],
  imports: [
    CommonModule, FormsModule, CurrencyPipe,
    DialogModule, TableModule, ButtonModule, TooltipModule,
    InputTextModule, InputNumberModule, FloatLabelModule,
    ToastModule
  ]
})
export class ProductosComponent {
  productos: Producto[] = [];
  modo: 'nuevo' | 'editar' = 'nuevo';
  dialogoVisible = false;
  form: Producto = this.vacio();

  filtro = {
    nombre: '',
    categoria: ''
  };
  productosFiltrados: Producto[] = [];

  constructor(
    private productosSrv: ProductoService,
    private toast: MessageService
  ) {
    this.cargar();
  }

  private vacio(): Producto {
    return { nombre: '', descripcion: '', categoria: '', imagenUrl: '', precio: 0, stock: 0 };
  }

  private cargar(): void {
  this.productosSrv.getAll().subscribe({
    next: d => {
      this.productos = d;
      this.filtrar(); // Aplica filtros cada vez que se carga
    },
    error: e => this.errorToast('Error al cargar productos', e)
  });
}

filtrar(): void {
  const nombre = this.filtro.nombre.toLowerCase();
  const categoria = this.filtro.categoria.toLowerCase();

  this.productosFiltrados = this.productos.filter(p =>
    (!nombre || p.nombre.toLowerCase().includes(nombre)) &&
    (!categoria || p.categoria?.toLowerCase().includes(categoria))
  );
}


  abrirNuevo() {
    this.modo = 'nuevo';
    this.form = this.vacio();
    this.dialogoVisible = true;
  }

  editar(p: Producto) {
    this.modo = 'editar';
    this.form = { ...p };
    this.dialogoVisible = true;
  }

  guardar(): void {
    const { nombre, descripcion, categoria, imagenUrl, precio, stock } = this.form;

    if (!nombre || !descripcion || !categoria || !imagenUrl || precio == null || stock == null || precio < 0 || stock < 0) {
      this.toast.add({
        severity: 'warn',
        summary: 'Campos requeridos',
        detail: 'Todos los campos son obligatorios y deben ser válidos.'
      });
      return;
    }

    this.form.precio = Number(precio);
    this.form.stock = Number(stock);

    const accion: Observable<any> = this.modo === 'nuevo'
      ? this.productosSrv.create(this.form)
      : this.productosSrv.update(this.form.id!, this.form);

    accion.subscribe({
      next: () => {
        this.toast.add({
          severity: 'success',
          summary: 'Éxito',
          detail: `Producto ${this.modo === 'nuevo' ? 'creado' : 'actualizado'} correctamente.`
        });
        this.cerrarYCargar();
      },
      error: e => this.errorToast(`Error al ${this.modo === 'nuevo' ? 'crear' : 'actualizar'} producto`, e)
    });
  }



  eliminar(id?: number | string) {
    if (!id) return;
    this.productosSrv.delete(id).subscribe({
      next: () => {
        this.toast.add({ severity: 'success', summary: 'Eliminado', detail: 'Producto eliminado correctamente.' });
        this.cargar();
      },
      error: e => this.errorToast('Error al eliminar producto', e)
    });
  }

  

  cerrar() {
    this.dialogoVisible = false;
  }

  private cerrarYCargar() {
    this.cerrar();
    this.cargar();
  }

  private errorToast(mensaje: string, error: unknown) {
    console.error(error);
    this.toast.add({
      severity: 'error',
      summary: 'Error',
      detail: mensaje
    });
  }


  
}
