import React from 'react';
import Sidebar from '../Sidebar/Sidebar';
import { Outlet } from 'react-router-dom';
import clsx from 'clsx';
import styles from './MainLayout.module.scss';
const MainLayout = () => {
  return (
    <div className={clsx(styles.div)}>
      <Sidebar />
      <main className={clsx(styles.main)}>
        <Outlet />
      </main>
    </div >
  );
};

export default MainLayout; 