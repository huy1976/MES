import React from 'react';
import clsx from 'clsx';
import styles from './LoadingPage.module.scss';
const LoadingPage = () => {
  return (
    <div className={clsx(styles.div)} style={styles}>
      <div className={clsx(styles.loader)}></div>
      <div style={{ height: 30 }}></div>
      <h2>Đang tải dữ liệu</h2>
    </div>
  );
};

export default LoadingPage; 